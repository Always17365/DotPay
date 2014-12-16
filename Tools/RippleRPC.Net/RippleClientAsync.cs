using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RippleRPC.Net.Exceptions;
using RippleRPC.Net.Infrastructure;
using RippleRPC.Net.Model;
using RippleRPC.Net.Model.Paths;
using WebSocket4Net;
using System.Linq;
using System.Linq.Expressions;
using FC.Framework;
using System.Threading;
using System.Collections.Concurrent;

namespace RippleRPC.Net
{
    public class RippleClientAsync : IDisposable, IRippleClientAsync
    {
        #region RPC Call ID
        private static int rpcId = 0;
        private int RPCCallId
        {
            get
            {
                return Interlocked.Increment(ref rpcId);
            }
        }
        #endregion

        #region RippleClient Config
        public bool AutoReconnect { get; set; }
        public bool WebSocketConnected { get { return WebSocketRL.State == WebSocketState.Open; } }
        public int TimeOut { get; set; }

        public Uri Uri { get; set; }
        public NetworkCredential Credentials { get; set; }
        private static WebSocket WebSocketRL { get; set; }

        private const string UserAgent = "Ripple.NET 0.1";
        #endregion

        #region RPCStack
        private ConcurrentDictionary<int, RippleRequestContext> RPCRequestList = new ConcurrentDictionary<int, RippleRequestContext>();

        #endregion

        #region Web Socket Callback
        private void WebScoketRL_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Log.Info("收到websocket返回数据:" + e.Message);
            var rippleError = default(RippleError);
            object resT = null;

            JObject jObject = JObject.Parse(e.Message);
            /*-----------------------------out error-------------------------------*/
            var resultToken = jObject.SelectToken("error", false);
            var id = default(JToken);

            if (resultToken != null)
            {
                rippleError = jObject.ToObject<RippleError>();
                var request = jObject.SelectToken("request", false);
                if (request != null && request.HasValues)
                    id = request.SelectToken("id", false);
            }
            else
            {
                resultToken = jObject.SelectToken("result", false);

                JValue status = resultToken["status"] as JValue;
                id = jObject.SelectToken("id", false);
                if (status != null && string.Compare(status.Value.ToString(), "error", StringComparison.InvariantCulture) == 0)
                {
                    rippleError = resultToken.ToObject<RippleError>();
                }
            }

            if (this.RPCRequestList.Count == 0)
                return;
            else
            {
                var _id = 0;

                if (id != null) _id = id.Value<int>();
                var reqContext = default(RippleRequestContext);

                if (_id > 0 && this.RPCRequestList.TryGetValue(_id, out reqContext))
                {
                    if (rippleError == null)
                    {
                        if (!string.IsNullOrEmpty(reqContext.DataNodeName))
                            resT = resultToken.SelectToken(reqContext.DataNodeName, false).ToObject(reqContext.DataType);
                        else
                            resT = resultToken.ToObject(reqContext.DataType);

                        Marker marker = null;
                        JToken markerToken = resultToken["marker"];

                        if (markerToken != null)
                        {
                            marker = markerToken.ToObject<Marker>();
                        }

                        //如果获取钱包的历史交易记录
                        if (reqContext.OriginRequest.command.Equals("account_tx", StringComparison.OrdinalIgnoreCase) && marker != null)
                        {
                            var list_tx_record = resT as List<TransactionRecord>;

                            if (reqContext.DataBuffer != null)
                                reqContext.DataBuffer.AddRange(list_tx_record);
                            else
                                reqContext.DataBuffer = list_tx_record;

                            //如果一次（在一个ledger中），未读取完所有的交易记录,重新发起请求
                            if (marker != null)
                            {
                                //修改请求中的滑动参数marker 
                                dynamic _params = reqContext.OriginRequest.requestParams;
                                _params.marker = marker;

                                if (this.WebSocketConnected)
                                    WebSocketRL.Send(reqContext.OriginRequest.ToJsonString());
                            }
                        }
                    }

                    //如果已读取到所有的记录或本次调用已完成，则调用回调函数，结束调用
                    reqContext.Error = rippleError;
                    reqContext.IsComplete = true;
                    reqContext.Data = reqContext.DataBuffer ?? resT;
                    reqContext.DataBuffer = null;
                    reqContext.ResetEvent.Set();

                }
            }
        }
        void WebScoketRL_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Log.Error("websocket 发生错误:" + e.Exception.Message, e.Exception);

            //如果已断开连接，则尝试重新连接
            if (WebSocketRL.State != WebSocketState.Open && WebSocketRL.State != WebSocketState.Connecting)
            {
                Task.Factory.StartNew(() =>
                {
                    if (this.AutoReconnect == true)
                    {
                        if (WebSocketRL.State != WebSocketState.Open && WebSocketRL.State != WebSocketState.Connecting)
                        {
                            WebSocketRL.Open();
                        }
                    }
                });
            }
        }

        void WebScoketRL_Closed(object sender, EventArgs e)
        {
            Log.Error("websocket 连接断开");

            Task.Factory.StartNew(() =>
            {
                if (this.AutoReconnect == true)
                {
                    if (WebSocketRL.State != WebSocketState.Open && WebSocketRL.State != WebSocketState.Connecting)
                    {
                        WebSocketRL.Open();
                    }
                }
            });
        }

        #endregion

        private RippleClientAsync()
        {
            //If you are using a self-signed certificate on the rippled server, retain the line below, otherwise you'll generate an exception as the certificate cannot be truested.
            //If you are using a purchased or trusted certificate, you can comment out this line.
            //ServicePointManager.ServerCertificateValidationCallback +=
            //    (s, cert, chain, sslPolicyErrors) => true;
            //ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack; 
        }

        public RippleClientAsync(Uri uri, int timeout = 10, bool autoReconnect = false)
            : this()
        {
            CreateWebsocket(uri, timeout, autoReconnect);
        }

        public RippleClientAsync(Uri uri, NetworkCredential credentials)
            : this(uri)
        {
            Uri = uri;
            Credentials = credentials;
        }

        #region private methods

        private Task<Tuple<RippleError, T>> RpcRequest<T>(RippleRequest rippleRequest, string objectElement = null)
        {
            string jsonRequest = rippleRequest.ToJsonString();

            var autoEvent = new AutoResetEvent(false);

            //启动等待任务 
            return Task.Factory.StartNew((req) =>
            {
                var _req = req as RippleRequest;
                var reqContext = new RippleRequestContext(typeof(T), objectElement, rippleRequest, autoEvent);
                //直到添加成功为止
                while (!this.RPCRequestList.TryAdd(rippleRequest.id, reqContext))
                {
                    //空白代码块，无需执行任何处理
                }
                if (this.WebSocketConnected)
                {
                    WebSocketRL.Send(_req.ToJsonString());
                    reqContext.IsStarted = true;
                }
                else
                {
                    if (WebSocketRL.State == WebSocketState.Closed)
                        try { WebSocketRL.Open(); }
                        catch { }
                }

                Tuple<RippleError, T> result = new Tuple<RippleError, T>(new RippleError { Error = "Unknow Error", ErrorCode = 610 }, default(T));

                //如果任务正常返回
                if (autoEvent.WaitOne(this.TimeOut * 1000))
                {
                    var reqEntity = default(RippleRequestContext);

                    if (this.RPCRequestList.TryGetValue(_req.id, out reqEntity))
                    {
                        result = new Tuple<RippleError, T>(reqEntity.Error, (T)reqEntity.Data);

                        this.RPCRequestList.TryRemove(_req.id, out reqEntity);
                    }
                }
                //如果等待超时，则调用超时回调
                else
                {
                    result = new Tuple<RippleError, T>(new RippleError { Error = "Websocket TimeOut", ErrorCode = 604, Message = "Websocket TimeOut" }, default(T));
                }

                return result;

            }, rippleRequest);
        }

        #endregion

        public Task<Tuple<RippleError, AccountInformation>> GetAccountInformation(string account, uint? index = null, string ledgerHash = null, bool strict = false, string ledgerIndex = "current")
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account must be provided", "account");

            dynamic param = new ExpandoObject();
            param.account = account;

            if (index.HasValue)
                param.index = index.Value;

            if (!string.IsNullOrEmpty(ledgerHash))
                param.ledger_hash = ledgerHash;

            param.strict = strict;

            param.ledger_index = ledgerIndex;

            RippleRequest request = new RippleRequest(this.RPCCallId, "account_info", param);

            return RpcRequest<AccountInformation>(request, "account_data");
        }

        public Task<Tuple<RippleError, List<AccountLine>>> GetAccountLines(string account, string peer = null, string ledgerIndex = "current")
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account must be provided", "account");

            dynamic param = new ExpandoObject();
            param.account = account;

            if (!string.IsNullOrEmpty(peer))
                param.peer = peer;

            param.ledger_index = ledgerIndex;

            RippleRequest request = new RippleRequest(this.RPCCallId, "account_lines", param);

            return RpcRequest<List<AccountLine>>(request, "lines");
        }

        public Task<Tuple<RippleError, List<AccountOffer>>> GetAccountOffers(string account, int accountIndex = 0, string ledgerHash = null, string ledgerIndex = "current")
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account must be provided", "account");

            dynamic param = new ExpandoObject();
            param.account = account;
            param.account_index = accountIndex;

            if (!string.IsNullOrEmpty(ledgerHash))
                param.ledger_hash = ledgerHash;

            param.ledger_index = ledgerIndex;

            RippleRequest request = new RippleRequest(this.RPCCallId, "account_offers", param);

            return RpcRequest<List<AccountOffer>>(request, "offers");
        }

        public Task<Tuple<RippleError, List<TransactionRecord>>> GetTransactions(string account, int minimumLedgerIndex = -1, int maximumLedgerIndex = -1, bool binary = false,
            bool forward = false, int limit = 200, Marker marker = null)
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account must be provided", "account");

            dynamic param = new ExpandoObject();
            param.account = account;
            param.ledger_index_min = minimumLedgerIndex;
            param.ledger_index_max = maximumLedgerIndex;
            param.binary = binary;
            param.forward = forward;
            param.limit = limit;

            if (marker != null)
                param.marker = marker;

            RippleRequest request = new RippleRequest(this.RPCCallId, "account_tx", param);

            return RpcRequest<List<TransactionRecord>>(request, "transactions");
        }


        public Task<Tuple<RippleError, List<TransactionRecord>>> GetTransactions(string account, int ledgerIndex, bool binary = false,
            bool forward = false, int limit = 200, Marker marker = null)
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account must be provided", "account");

            dynamic param = new ExpandoObject();
            param.account = account;
            param.ledger_index = ledgerIndex;
            param.binary = binary;
            param.forward = forward;
            param.limit = limit;

            if (marker != null)
                param.marker = marker;

            RippleRequest request = new RippleRequest(this.RPCCallId, "account_tx", param);

            return RpcRequest<List<TransactionRecord>>(request, "transactions");
        }

        public Task<Tuple<RippleError, List<BookOffer>>> GetBookOffers(RippleCurrency takerPays, RippleCurrency takerGets, string ledger = "current", string taker = null, int limit = 200,
            bool proof = false, bool autoBridge = false, Marker marker = null)
        {
            if (takerPays == null)
                throw new ArgumentException("Taker pays must be provided", "takerPays");

            if (takerGets == null)
                throw new ArgumentException("Taker gets must be provided", "takerGets");

            dynamic param = new ExpandoObject();
            param.taker_pays = takerPays;
            param.taker_gets = takerGets;
            if (!string.IsNullOrEmpty(taker))
                param.taker_gets = taker;
            param.limit = limit;
            param.proof = proof;
            param.autobridge = autoBridge;

            List<BookOffer> transactions = new List<BookOffer>();

            if (marker != null)
                param.marker = marker;
            RippleRequest request = new RippleRequest(this.RPCCallId, "book_offers", param);

            return RpcRequest<List<BookOffer>>(request, "offers");
        }

        //public LedgerSummary GetLedgerInformation(string ledgerIndex = "current", bool full = true)
        //{
        //    dynamic param = new ExpandoObject();
        //    param.ledger_selector = ledgerIndex;
        //    param.full = full;

        //    RippleRequest request = new RippleRequest(this.RPCCallId, "ledger", param);

        //    string jsonRequest = JsonConvert.SerializeObject(request);
        //    string response = ""; //MakeRequest(jsonRequest);

        //    JObject jObject = JObject.Parse(response);

        //    var resultToken = jObject.SelectToken("result", false);

        //    JValue status = resultToken["status"] as JValue;
        //    if (status != null && string.Compare(status.Value.ToString(), "error", StringComparison.InvariantCulture) == 0)
        //    {
        //        RippleError rippleError = resultToken.ToObject<RippleError>();
        //        throw new RippleRpcException(rippleError); ;
        //    }

        //    LedgerSummary summary = new LedgerSummary();
        //    summary.Open = resultToken.SelectToken("open", false).ToObject<OpenLedger>();
        //    summary.Closed = resultToken.SelectToken("closed", false).ToObject<ClosedLedger>();

        //    return summary;

        //}

        public Task<Tuple<RippleError, string>> GetClosedLedgerHash()
        {
            RippleRequest request = new RippleRequest(this.RPCCallId, "ledger_closed", new ExpandoObject());

            return RpcRequest<string>(request, "ledger_hash");
        }

        public Task<Tuple<RippleError, int>> GetCurrentLedgerIndex()
        {
            RippleRequest request = new RippleRequest(this.RPCCallId, "ledger_current", new ExpandoObject());

            return RpcRequest<int>(request, "ledger_current_index");
        }

        public Task<Tuple<RippleError, PathSummary>> RipplePathFind(string fromAccount, string toAccount, RippleCurrencyValue destinationamount)
        {
            if (string.IsNullOrEmpty(fromAccount))
                throw new ArgumentException("From account must be provided", "fromAccount");

            if (string.IsNullOrEmpty(toAccount))
                throw new ArgumentException("To account must be provided", "toAccount");

            dynamic param = new ExpandoObject();

            //if (currency != null && currency.Length > 0)
            //{
            //    var currencyList = new List<object>();

            //    currency.ForEach(c =>
            //    {
            //        currencyList.Add(new { currency = c });
            //    });

            //    param.source_currencies = currencyList;

            //}

            param.source_account = fromAccount;
            param.destination_account = toAccount;
            param.destination_amount = destinationamount;

            RippleRequest request = new RippleRequest(this.RPCCallId, "ripple_path_find", param);

            return RpcRequest<PathSummary>(request, null);
        }

        public Task<Tuple<RippleError, PaymentSubmitResult>> SendXRP(string fromAccount, string secret, string toAccount, decimal amount)
        {
            var transaction = new Transaction { Flags = TransactionFlags.tfFullyCanonicalSig, DestinationTag = 110, Fee = 10000, TransactionType = TransactionType.Payment, Account = fromAccount, Destination = toAccount, Amount = new RippleCurrencyValue { Currency = "XRP", Issuer = string.Empty, _Value = amount.ToString() } };

            return this.Sign(transaction, secret)
                       .ContinueWith<Tuple<RippleError, PaymentSubmitResult>>((signTask) =>
                       {
                           if (signTask.Result.Item1 == null)
                           {
                               var _signedTx = signTask.Result.Item2.TxBlob;
                               return this.Submit(_signedTx).Result;
                           }
                           else
                           {
                               return new Tuple<RippleError, PaymentSubmitResult>(signTask.Result.Item1, default(PaymentSubmitResult));
                           }
                       });
        }

        public Task<Tuple<RippleError, PaymentSubmitResult>> Submit(string transactionHash)
        {
            if (string.IsNullOrEmpty(transactionHash))
                throw new ArgumentException("Transaction hash cannot be null", "transactionHash");
            dynamic param = new ExpandoObject();
            param.tx_blob = transactionHash;

            RippleRequest request = new RippleRequest(this.RPCCallId, "submit", param);

            return RpcRequest<PaymentSubmitResult>(request, null);
        }

        public Task<Tuple<RippleError, TransactionSigned>> Sign(Transaction transaction, string secret, bool offline = false)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("secret must be provided", "secret");

            if (transaction == null)
                throw new ArgumentException("transaction must be provided", "transaction");

            //if (offline)
            //{
            //    var accountInfo = GetAccountInformation(transaction.Account).Result;
            //    transaction.Sequence = accountInfo.Item2.Sequence + 1;
            //    transaction.Sign(secret);
            //    hash = transaction.Hash;
            //}
            //else
            //{
            dynamic param = new ExpandoObject();
            param.offline = offline;
            param.secret = secret;
            param.tx_json = transaction;

            RippleRequest request = new RippleRequest(this.RPCCallId, "sign", param);

            return RpcRequest<TransactionSigned>(request);
            //}
        }

        private static bool CertificateValidationCallBack(object sender,
           System.Security.Cryptography.X509Certificates.X509Certificate certificate,
           System.Security.Cryptography.X509Certificates.X509Chain chain,
           System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) && (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            if (certificate.Subject == "O=Internet Widgits Pty Ltd, S=Some-State, C=AU")
                                continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            // In all other cases, return false.
            return false;
        }

        private void CreateWebsocket(Uri uri, int timeout, bool autoReconnect)
        {
            this.Uri = uri;
            if (WebSocketRL == null)
            {
                WebSocketRL = new WebSocket(uri.ToString());
                WebSocketRL.Opened += (s, e) =>
                {
                    if (this.RPCRequestList.Count(req => !req.Value.IsStarted) > 0)
                    {
                        RPCRequestList.Where(req => !req.Value.IsStarted).ForEach(wct =>
                        {
                            WebSocketRL.Send(wct.Value.OriginRequest.ToJsonString());
                            wct.Value.IsStarted = true;
                        });
                    }
                };
                this.TimeOut = timeout;
                this.AutoReconnect = autoReconnect;
                WebSocketRL.Error += WebScoketRL_Error;
                WebSocketRL.Closed += WebScoketRL_Closed;
                WebSocketRL.MessageReceived += WebScoketRL_MessageReceived;
                WebSocketRL.Open();
            }
        }

        public void Dispose()
        {
            this.AutoReconnect = false;
            WebSocketRL.Close(200, "close by code");
        }

        public class RippleRequestContext
        {
            public RippleRequestContext(Type dataType, string dataNodeName, RippleRequest request, AutoResetEvent autoResetEvent)
            {
                this.DataType = dataType;
                this.DataNodeName = dataNodeName;
                this.OriginRequest = request;
                this.ResetEvent = autoResetEvent;
            }
            /// <summary>
            /// 返回的数据
            /// </summary>
            public object Data { get; set; }
            /// <summary>
            /// 返回数据的实际类型
            /// </summary>
            public Type DataType { get; private set; }
            /// <summary>
            /// 返回数据需要读取json特定节点的名称
            /// </summary>
            public string DataNodeName { get; private set; }

            /// <summary>
            /// 本次请求是否已经发送
            /// </summary>
            public bool IsStarted { get; set; }
            /// <summary>
            /// 本次请求是否已经完成
            /// </summary>
            public bool IsComplete { get; set; }
            /// <summary>
            /// 请求调用返回的错误信息
            /// </summary>
            public RippleError Error { get; set; }
            /// <summary>
            /// 数据缓冲(account_tx调用，有时会用到)
            /// </summary>
            public List<TransactionRecord> DataBuffer { get; set; }

            /// <summary>
            /// 本次原始请求
            /// </summary>
            public RippleRequest OriginRequest { get; private set; }
            /// <summary>
            /// 同步Event
            /// </summary>
            public AutoResetEvent ResetEvent { get; private set; }
        }

    }
}
