using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;
using MongoDB.Bson;
using MongoDB.Driver;
using DFramework;
using Dotpay.Common.Enum;

namespace Dotpay.FrontQueryServiceImpl
{
    [QueryService]
    public class TransactionQueryImpl : ITransactionQuery
    {
        private const string DEPOSIT_COLLECTION_NAME = "Dotpay.Actor.Implementations.DepositTransaction";
        private const string TRANSFER_COLLECTION_NAME = "Dotpay.Actor.Implementations.TransferTransaction";
        public async Task<IEnumerable<IndexTransactionListViewModel>> GetLastTenTransationByAccountId(Guid accountId/*, DateTime start, DateTime end*/)
        {
            var result = new List<IndexTransactionListViewModel>();
            var depositCollection = MongoManager.GetCollection<BsonDocument>(DEPOSIT_COLLECTION_NAME);
            var transferCollection = MongoManager.GetCollection<BsonDocument>(TRANSFER_COLLECTION_NAME);

            var depositFilter = new BsonDocument("AccountId", accountId.ToString());
            var transferFilter = new BsonDocument("TransactionInfo.Source.AccountId", accountId.ToString());
            var depositProjection = BsonDocument.Parse("{Currency:1,SequenceNo:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1,_id:0}");
            var transferProjection = BsonDocument.Parse("{TransactionInfo:1,SequenceNo:1,RippleTransactionInfo:1,Currency:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1,Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:-1}");
            var depositOptions = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 10,
                Projection = depositProjection,
                Sort = sort
            };
            var transferOptions = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 10,
                Projection = transferProjection,
                Sort = sort
            };

            var depositQuery = depositCollection.FindAsync<BsonDocument>(depositFilter, depositOptions);
            var transferQuery = transferCollection.FindAsync<BsonDocument>(transferFilter, transferOptions);

            var queryResults = await Task.WhenAll(depositQuery, transferQuery);

            for (int i = 0; i < queryResults.Length; i++)
            {
                var cursor = queryResults[i];
                using (cursor)
                {
                    var txs = await cursor.ToListAsync();

                    if (txs.Any())
                    {
                        //充值
                        if (i == 0)
                        {

                            txs.ForEach(row =>
                               {
                                   var item = new IndexTransactionListViewModel()
                                   {
                                       Type = "DepositTransaction",
                                       SequenceNo = row["SequenceNo"].AsString,
                                       Currency = ((CurrencyType)row["Currency"].AsInt32).ToLangString(),
                                       Amount = (decimal)row["Amount"].AsDouble,
                                       Status = ((DepositStatus)row["Status"].AsInt32).ToLangString(),
                                       Payway = ((Payway)row["Payway"].AsInt32).ToLangString(),
                                       CreateAt = row["CreateAt"].AsDouble.ToLocalDateTime(),
                                       Memo = row.GetValue("Memo", BsonValue.Create("")).AsString,
                                       Reason = row.GetValue("Reason", BsonValue.Create("")).AsString
                                   };
                                   result.Add(item);
                               });
                        }
                        //转账
                        else
                        {
                            txs.ForEach(row =>
                            {
                                var txinfoString = row["TransactionInfo"].ToJson();
                                var txTargetString = row["TransactionInfo"]["Target"].ToJson();
                                var txinfo = IoC.Resolve<IJsonSerializer>().Deserialize<TransferTransactionInfo>(txinfoString);


                                var item = new IndexTransactionListViewModel()
                                {
                                    Type = "TransferTransaction",
                                    SequenceNo = row["SequenceNo"].AsString,
                                    Currency = txinfo.Currency.ToLangString(),
                                    Amount = txinfo.Amount,
                                    Status = ((TransferTransactionStatus)row["Status"].AsInt32).ToLangString(),
                                    Payway = txinfo.Target.Payway.ToLangString(),
                                    CreateAt = row["CreateAt"].AsDouble.ToLocalDateTime(),
                                    Memo = row.GetValue("Memo", BsonValue.Create("")).AsString,
                                    Reason = row.GetValue("Reason", BsonValue.Create("")).AsString
                                };

                                switch (txinfo.Target.Payway)
                                {
                                    case Payway.Dotpay:
                                        var toDotpay =
                                            IoC.Resolve<IJsonSerializer>()
                                                .Deserialize<TransferToDotpayTargetInfo>(txTargetString);
                                        item.Destination = !string.IsNullOrEmpty(toDotpay.RealName)
                                            ? toDotpay.RealName
                                            : !string.IsNullOrEmpty(toDotpay.UserLoginName)
                                                ? toDotpay.UserLoginName
                                                : toDotpay.Email;

                                        break;
                                    case Payway.Bank:
                                        var toBank =
                                            IoC.Resolve<IJsonSerializer>()
                                                .Deserialize<TransferToBankTargetInfo>(txTargetString);
                                        item.Destination = toBank.RealName;
                                        break;
                                    case Payway.Ripple:
                                        var toRipple =
                                            IoC.Resolve<IJsonSerializer>()
                                                .Deserialize<TransferToRippleTargetInfo>(txTargetString);

                                        item.Destination = toRipple.Destination;
                                        break;
                                    case Payway.Alipay:
                                    case Payway.Tenpay:
                                        var toTpp =
                                            IoC.Resolve<IJsonSerializer>()
                                                .Deserialize<TransferToTppTargetInfo>(txTargetString);
                                        item.Destination = !string.IsNullOrEmpty(toTpp.RealName)
                                            ? toTpp.RealName
                                            : toTpp.DestinationAccount;
                                        break;
                                }
                                result.Add(item);
                            });
                        }
                    }
                }
            }

            return result.OrderByDescending(r => r.CreateAt).Take(10);
        }

        public async Task<int> CountDepositTransaction(Guid accountId, DateTime start, DateTime end)
        {
            var condation = "{ AccountId : '" + accountId.ToString() + "'" +
                            " ,Status:{ $in:[" + TransferTransactionStatus.Submited.ToString("D") +
                                   "," + TransferTransactionStatus.LockeByProcessor.ToString("D") + "]}}";
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(DEPOSIT_COLLECTION_NAME);
            var filter = BsonDocument.Parse(condation);

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public Task<IEnumerable<DepositTransactionListViewModel>> GetDepositTransaction(Guid accountId, DateTime start, DateTime end, int page, int pagesize)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountTransferTransaction(Guid accountId, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransferTransactionListViewModel>> GetTransferTransaction(Guid accountId, DateTime start, DateTime end, int page, int pagesize)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> GetDepositTransactionIdBySeqNo(string sequenceNo)
        {
            Guid result = Guid.Empty;
            var collection = MongoManager.GetCollection<BsonDocument>(DEPOSIT_COLLECTION_NAME);

            var filter = new BsonDocument("SequenceNo", sequenceNo);
            var projection = BsonDocument.Parse("{Id:1,_id:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1,
                Projection = projection
            };

            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                var row = results.FirstOrDefault();
                if (row != null)
                    result = Guid.Parse(row["Id"].AsString);
            }
            return result;
        }

        public async Task<TransferTransactionDetailViewModel> GetTransferTransactionBySeqNo(string sequenceNo)
        {
            TransferTransactionDetailViewModel result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(TRANSFER_COLLECTION_NAME);

            var filter = new BsonDocument("SequenceNo", sequenceNo);
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1 
            };

            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                var row = results.FirstOrDefault();
                if (row != null)
                {

                    row.Remove("_id");
                    var json = row.ToJson();
                    var txTargetString = row["TransactionInfo"]["Target"].ToJson();
                    var payway = (Payway)row["TransactionInfo"]["Target"]["Payway"].AsInt32;
                    result = new TransferTransactionDetailViewModel()
                    {
                        TransferUserId = Guid.Parse(row["TransactionInfo"]["Source"]["UserId"].AsString),
                        SequenceNo = row["SequenceNo"].AsString,
                        Amount = (decimal)row["TransactionInfo"]["Amount"].AsDouble,
                        Currency = ((CurrencyType)row["TransactionInfo"]["Currency"].AsInt32),
                        Payway = payway,
                        Memo = row["TransactionInfo"].AsBsonDocument.GetValue("Memo", "").AsString
                    };
                    switch (payway)
                    {
                        case Payway.Dotpay:
                            var toDotpay =
                                IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferToDotpayTargetInfo>(txTargetString);
                            result.Destination = !string.IsNullOrEmpty(toDotpay.RealName)
                                ? toDotpay.RealName
                                : !string.IsNullOrEmpty(toDotpay.UserLoginName)
                                    ? toDotpay.UserLoginName
                                    : toDotpay.Email;

                            break;
                        case Payway.Bank:
                            var toBank =
                                IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferToBankTargetInfo>(txTargetString);
                            result.Destination = toBank.RealName;
                            result.Bank = toBank.Bank;
                            break;
                        case Payway.Ripple:
                            var toRipple =
                                IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferToRippleTargetInfo>(txTargetString);

                            result.Destination = toRipple.Destination;
                            break;
                        case Payway.Alipay:
                        case Payway.Tenpay:
                            var toTpp =
                                IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferToTppTargetInfo>(txTargetString);
                            result.Destination = !string.IsNullOrEmpty(toTpp.RealName)
                                ? toTpp.RealName
                                : toTpp.DestinationAccount;
                            break;
                    }
                }
            }
            return result;
        }
    }
}
