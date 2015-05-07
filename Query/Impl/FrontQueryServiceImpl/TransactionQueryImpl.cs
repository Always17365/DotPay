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
            var depositProjection = BsonDocument.Parse("{Id:1,Currency:1,SequenceNo:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1,_id:0}");
            var transferProjection = BsonDocument.Parse("{Id:1,TransactionInfo:1,SequenceNo:1,RippleTransactionInfo:1,Currency:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1,Reason:1,_id:0}");
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
                                       Id = row["Id"].AsString,
                                       Type = "DepositTransaction",
                                       SequenceNo = row["SequenceNo"].AsString,
                                       Currency = ((CurrencyType)row["Currency"].AsInt32).ToLangString(),
                                       Amount = (decimal)row["Amount"].AsDouble,
                                       Status = ((DepositStatus)row["Status"].AsInt32).ToLangString(),
                                       Payway = (Payway)row["Payway"].AsInt32,
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
                                    Id = row["Id"].AsString,
                                    Type = "TransferTransaction",
                                    SequenceNo = row["SequenceNo"].AsString,
                                    Currency = txinfo.Currency.ToLangString(),
                                    Amount = txinfo.Amount,
                                    Status = ((TransferTransactionStatus)row["Status"].AsInt32).ToLangString(),
                                    Payway = txinfo.Target.Payway,
                                    CreateAt = row["CreateAt"].AsDouble.ToLocalDateTime(),
                                    Memo = txinfo.Memo,
                                    Reason = row.GetValue("Reason", BsonValue.Create("")).AsString
                                };

                                var targetInfo =
                                            IoC.Resolve<IJsonSerializer>()
                                                .Deserialize<TransferTargetInfo>(txTargetString);
                                switch (txinfo.Target.Payway)
                                {
                                    case Payway.Dotpay:
                                        item.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                            ? targetInfo.RealName
                                            : !string.IsNullOrEmpty(targetInfo.UserLoginName)
                                                ? targetInfo.UserLoginName
                                                : targetInfo.Email;

                                        break;
                                    case Payway.Bank:
                                        item.Bank = targetInfo.Bank;
                                        item.Destination = targetInfo.RealName;
                                        break;
                                    case Payway.Ripple:
                                        item.Destination = targetInfo.Destination;
                                        break;
                                    case Payway.Alipay:
                                    case Payway.Tenpay:
                                        item.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                            ? targetInfo.RealName
                                            : targetInfo.Destination;
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

        public async Task<decimal> SumDepositAmount(Guid accountId, DateTime start, DateTime end)
        {
            decimal result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(DEPOSIT_COLLECTION_NAME);

            var condation1 = "AccountId : '" + accountId.ToString().ToLower() + "'";
            var condation2 = "CreateAt:{ $gte:" + start.ToDoubleUnixTimestamp() + ",$lte:" + end.ToDoubleUnixTimestamp() + "}";
            var condation3 = "Status: " + DepositStatus.Completed.ToString("D");

            var condations = new[] { condation1, condation2, condation3 };

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");
            string map = @"function(){emit(this.Id,this.Amount);};";

            string reduce = @"function(id,amounts){  
                                    var total = 0;  
                                    total = Array.sum(amounts);  
                                    return {id:id,sum: total };  
                                   };";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = filter
            };
            try
            {
                using (var cursor = await collection.MapReduceAsync<BsonDocument>(map, reduce, options))
                {
                    var results = await cursor.ToListAsync();

                    var row = results.FirstOrDefault();
                    if (row != null)
                    {
                        if (row["value"].IsBsonDocument)
                        {
                            result = (Decimal)row["value"].AsBsonDocument.GetValue("sum", 0).ToDouble();
                        }
                        else
                            result = (Decimal)row.GetValue("value", 0).ToDouble();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<Tuple<decimal, decimal>> SumTransferOutAndInAmount(Guid accountId, DateTime start, DateTime end)
        {
            decimal @out = 0, @in = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(TRANSFER_COLLECTION_NAME);

            #region out
            var condation1 = "'TransactionInfo.Source.AccountId' : '" + accountId.ToString().ToLower() + "'";
            var condation2 = "CreateAt:{ $gte:" + start.ToDoubleUnixTimestamp() + ",$lte:" + end.ToDoubleUnixTimestamp() + "}";
            var condation3 = "Status: { $in:[" + TransferTransactionStatus.LockeByProcessor.ToString("D") + "," +
                                                 TransferTransactionStatus.PreparationCompleted.ToString("D") + "," +
                                                 TransferTransactionStatus.Completed.ToString("D") + "]}";

            var condations = new[] { condation1, condation2, condation3 };

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");
            string map = @"function(){emit(this.TransactionInfo.Source.AccountId,this.TransactionInfo.Amount);};";

            string reduce = @"function(id,amounts){  
                                    var total = 0;  
                                    total = Array.sum(amounts);  
                                    return {id:id,sum: total };  
                                   };";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = filter
            };
            try
            {
                using (var cursor = await collection.MapReduceAsync<BsonDocument>(map, reduce, options))
                {
                    var results = await cursor.ToListAsync();

                    var row = results.FirstOrDefault();
                    if (row != null)
                    {
                        if (row["value"].IsBsonDocument)
                        {
                            @out = (Decimal)row["value"].AsBsonDocument.GetValue("sum", 0).ToDouble();
                        }
                        else
                            @out = (Decimal)row.GetValue("value", 0).ToDouble();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            #endregion

            #region in
            var condation1In = "'TransactionInfo.Target.AccountId': '" + accountId.ToString().ToLower() + "'";
            var condation2In = "CreateAt:{ $gte:" + start.ToDoubleUnixTimestamp() + ",$lte:" + end.ToDoubleUnixTimestamp() + "}";
            var condation3In = "Status: " + TransferTransactionStatus.Completed.ToString("D");

            var condationsIn = new[] { condation1In, condation2In, condation3In };

            var filterIn = BsonDocument.Parse("{" + string.Join(",", condationsIn.Where(c => !string.IsNullOrEmpty(c))) + " }");
            var mapIn = @"function(){emit(this.TransactionInfo.Target.AccountId,this.TransactionInfo.Amount);};";

            var reduceIn = @"function(id,amounts){  
                                    var total = 0;  
                                    total = Array.sum(amounts);  
                                    return {id:id,sum: total };  
                                   };";
            var options_in = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = filterIn
            };

            try
            {
                using (var cursor = await collection.MapReduceAsync<BsonDocument>(mapIn, reduceIn, options_in))
                {
                    var results = await cursor.ToListAsync();

                    var row = results.FirstOrDefault();
                    if (row != null)
                    {
                        if (row["value"].IsBsonDocument)
                        {
                            @in = (Decimal)row["value"].AsBsonDocument.GetValue("sum", 0).ToDouble();
                        }
                        else
                            @in = (Decimal)row.GetValue("value", 0).ToDouble();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            #endregion

            return new Tuple<decimal, decimal>(@out, @in);
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

        public async Task<TransferTransactionSubmitViewModel> GetTransferTransactionSubmitDetailByTxid(Guid txid)
        {
            TransferTransactionSubmitViewModel result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(TRANSFER_COLLECTION_NAME);

            var filter = new BsonDocument("Id", txid.ToString());
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
                    var targetBson = row["TransactionInfo"]["Target"].AsBsonDocument;
                    var bank = row["TransactionInfo"]["Target"].AsBsonDocument.GetValue("Bank", 0).AsInt32;
                    result = new TransferTransactionSubmitViewModel()
                    {
                        TransferUserId = Guid.Parse(row["TransactionInfo"]["Source"]["UserId"].AsString),
                        Amount = (decimal)row["TransactionInfo"]["Amount"].AsDouble,
                        Currency = ((CurrencyType)row["TransactionInfo"]["Currency"].AsInt32),
                        Payway = payway,
                        Memo = row["TransactionInfo"].AsBsonDocument.GetValue("Memo", "").AsString,
                        Bank = bank == 0 ? null : new Bank?((Bank)bank),
                        RealName = targetBson.GetValue("RealName", "").AsString,
                        TransferTransactionId = txid
                    };

                    var targetInfo = IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferTargetInfo>(txTargetString);
                    switch (payway)
                    {
                        case Payway.Dotpay:
                            result.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                ? targetInfo.RealName
                                : !string.IsNullOrEmpty(targetInfo.UserLoginName)
                                    ? targetInfo.UserLoginName
                                    : targetInfo.Email;
                            result.DestinationAccountId = targetInfo.AccountId;
                            break;
                        case Payway.Bank:
                            result.Destination = targetInfo.RealName;
                            result.Bank = targetInfo.Bank;
                            break;
                        case Payway.Ripple:
                            result.Destination = targetInfo.Destination;
                            break;
                        case Payway.Alipay:
                        case Payway.Tenpay:
                            result.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                ? targetInfo.RealName
                                : targetInfo.Destination;
                            break;
                    }
                }
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
                    var targetInfo = IoC.Resolve<IJsonSerializer>()
                                    .Deserialize<TransferTargetInfo>(txTargetString);
                    switch (payway)
                    {
                        case Payway.Dotpay:
                            result.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                ? targetInfo.RealName
                                : !string.IsNullOrEmpty(targetInfo.UserLoginName)
                                    ? targetInfo.UserLoginName
                                    : targetInfo.Email;
                            result.DestinationAccountId = targetInfo.AccountId;
                            break;
                        case Payway.Bank:
                            result.Destination = targetInfo.RealName;
                            result.Bank = targetInfo.Bank;
                            break;
                        case Payway.Ripple:
                            result.Destination = targetInfo.Destination;
                            break;
                        case Payway.Alipay:
                        case Payway.Tenpay:
                            result.Destination = !string.IsNullOrEmpty(targetInfo.RealName)
                                ? targetInfo.RealName
                                : targetInfo.Destination;
                            break;
                    }
                }
            }
            return result;
        }
    }
}
