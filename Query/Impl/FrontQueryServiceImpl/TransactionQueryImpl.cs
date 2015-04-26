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
        public async Task<IEnumerable<IndexTransactionListViewModel>> GetLastTenTransationByAccountId(Guid accountId, DateTime start, DateTime end)
        {
            var result = new List<IndexTransactionListViewModel>();
            var depositCollection = MongoManager.GetCollection<BsonDocument>(DEPOSIT_COLLECTION_NAME);
            var transferCollection = MongoManager.GetCollection<BsonDocument>(TRANSFER_COLLECTION_NAME);

            var filter = new BsonDocument("AccountId", accountId.ToString());
            var depositProjection = BsonDocument.Parse("{Currency:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1,_id:0}");
            var transferProjection = BsonDocument.Parse("{TransactionInfo:1,RippleTransactionInfo:1,Currency:1,Amount:1,Status:1,Payway:1,Memo:1,CreateAt:1.Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:0}");
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

            var depositQuery = depositCollection.FindAsync<BsonDocument>(filter, depositOptions);
            var transferQuery = transferCollection.FindAsync<BsonDocument>(filter, transferOptions);

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
                                       Currency = ((CurrencyType)row["Currency"].AsInt32).ToString("F"),
                                       Amount = (decimal)row["Amount"].AsDouble,
                                       Status = ((DepositStatus)row["Status"].AsInt32).ToString("F"),
                                       Payway = ((Payway)row["Payway"].AsInt32).ToString("F"),
                                       CreateAt = Convert.ToDateTime(row["CreateAt"].AsString),
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
                                var txinfoString = row["TransferTransactionInfo"].AsString;
                                var txinfo = IoC.Resolve<TransferTransactionInfo>(txinfoString);

                                var item = new IndexTransactionListViewModel()
                                {
                                    Type = "TransferTransaction",
                                    Currency = txinfo.Currency.ToString("F"),
                                    Amount = txinfo.Amount,
                                    Status = ((TransferTransactionStatus)row["Status"].AsInt32).ToString("F"),
                                    Payway = txinfo.Target.Payway.ToString("F"),
                                    CreateAt = Convert.ToDateTime(row["CreateAt"].AsString),
                                    Memo = row.GetValue("Memo", BsonValue.Create("")).AsString,
                                    Reason = row.GetValue("Reason", BsonValue.Create("")).AsString
                                };
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
                            "Status:{ $gte:[" + TransferTransactionStatus.Submited.ToString("D") +
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


    }
}
