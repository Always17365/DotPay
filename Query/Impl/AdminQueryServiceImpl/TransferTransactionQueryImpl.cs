using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminQueryService;
using Dotpay.Common.Enum;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dotpay.AdminQueryServiceImpl
{
    [QueryService]
    public class TransferTransactionQueryImpl : ITransferTransactionQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.TransferTransaction";
        public async Task<int> CountPendingDotpayToFiTransferTx(string userLoginName)
        {
            var condation = "{" +
                           (string.IsNullOrEmpty(userLoginName)
                               ? ""
                               : "UserLoginName : { $regex : '" + userLoginName + "', $options : 'i' },") +
                                 "Status:{ $in:[" + TransferTransactionStatus.Submited.ToString("D") +
                                    "," + TransferTransactionStatus.LockeByProcessor.ToString("D") + "]}}";
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = BsonDocument.Parse(condation);

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetPendingDotpayToFiTransferTx(string userLoginName, int start, int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            IEnumerable<TransferFromDotpayToFiListViewModel> result = null;
            var condation = "{" +
                         (string.IsNullOrEmpty(userLoginName) ? "" : "UserLoginName : { $regex : '" + userLoginName + "', $options : 'i' },") +
                               "Status:{ $in:[" + TransferTransactionStatus.Submited.ToString("D") +
                                    "," + TransferTransactionStatus.LockeByProcessor.ToString("D") + "]}}";

            var filter = BsonDocument.Parse(condation);

            var projection = BsonDocument.Parse("{Id:1,SequenceNo:1,TransactionInfo:1,ManagerId:1,CreateAt:1,CompleteAt:1,FailAt:1,Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = pagesize,
                Projection = projection,
                Skip = start,
                Sort = sort
            };
            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                if (results.Any())
                {
                    result = new List<TransferFromDotpayToFiListViewModel>();
                    results.ForEach(r =>
                    {
                        var bank = ((Bank?)r["TransferTransactionInfo"]["Target"]["Bank"].AsNullableInt32);
                        var item = new TransferFromDotpayToFiListViewModel()
                        {
                            Id = r["Id"].AsGuid,
                            UserLoginName = r["TransferTransactionInfo"]["Source"]["UserLoginName"] != null ? r["TransferTransactionInfo"]["Source"]["UserLoginName"].AsString : null,
                            SequenceNo = r["SequenceNo"].AsString,
                            Currency = ((CurrencyType)r["TransferTransactionInfo"]["Currency"].ToInt32()).ToString("F"),
                            Amount = (decimal)r["TransferTransactionInfo"]["Amount"].ToDouble(),
                            Bank = bank.HasValue ? bank.Value.ToString("F") : "",
                            CreateAt = r["CreateAt"].ToLocalTime(),
                            DestinationAccount = r["TransferTransactionInfo"]["Target"]["DestinationAccount"].AsString,
                            Manager = r["Manager"] != null ? r["Manager"].AsString : null,
                            Payway = ((Payway)r["TransferTransactionInfo"]["Target"]["Payway"].AsInt32).ToString("F")
                        };
                    });
                }
            }
            return result;
        }

        public async Task<int> CountDotpayToFiTransferTx(string userLoginName, string sequenceNo, string transferNo, TransferTransactionStatus status)
        {
            var condation = "{" +
                            (string.IsNullOrEmpty(userLoginName) ? "" : "UserLoginName : { $regex : '" + userLoginName + "', $options : 'i' },") +
                            (string.IsNullOrEmpty(sequenceNo) ? "" : " SequenceNo:'" + sequenceNo + "',") +
                            (string.IsNullOrEmpty(transferNo) ? "" : " FiTransactionNo:'" + transferNo + "',") +
                            " Status:" + status.ToString("D") + "}";
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = BsonDocument.Parse(condation);

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetDotpayToFiTransferTx(
            string userLoginName, string sequenceNo, string transferNo, TransferTransactionStatus status, int start,
            int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            IEnumerable<TransferFromDotpayToFiListViewModel> result = null;
            var condation = "{" +
                            (string.IsNullOrEmpty(userLoginName) ? "" : "UserLoginName : { $regex : '" + userLoginName + "', $options : 'i' },") +
                            (string.IsNullOrEmpty(sequenceNo) ? "" : " SequenceNo:'" + sequenceNo + "',") +
                            (string.IsNullOrEmpty(transferNo) ? "" : " FiTransactionNo:'" + transferNo + "',") +
                            " Status:" + status.ToString("D") + "}";

            var filter = BsonDocument.Parse(condation);

            var projection =
                BsonDocument.Parse(
                    "{Id:1,SequenceNo:1,TransactionInfo:1,FiTransactionNo:1,ManagerId:1,CreateAt:1,CompleteAt:1,FailAt:1,Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = pagesize,
                Projection = projection,
                Skip = start,
                Sort = sort
            };
            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                if (results.Any())
                {
                    result = new List<TransferFromDotpayToFiListViewModel>();
                    results.ForEach(row =>
                    {
                        var bank = (Bank?)
                            (row["TransferTransactionInfo"]["Target"].AsBsonDocument.GetValue("Bank", BsonValue.Create(0)).ToInt32())
                        ;
                        var item = new TransferFromDotpayToFiListViewModel()
                        {
                            Id = Guid.Parse(row["Id"].AsString),
                            UserLoginName = row["TransferTransactionInfo"]["Source"].AsBsonDocument.GetValue("UserLoginName", BsonValue.Create("")).AsString,
                            SequenceNo = row["SequenceNo"].AsString,
                            Currency = ((CurrencyType)row["TransferTransactionInfo"]["Currency"].ToInt32()).ToString("F"),
                            Amount = (decimal)row["TransferTransactionInfo"]["Amount"].AsDouble,
                            Bank = bank.HasValue ? bank.Value.ToString("F") : "",
                            CreateAt = row.GetValue("CreateAt", 0d).AsDouble.ToLocalDateTime(),
                            DestinationAccount = row["TransferTransactionInfo"]["Target"]["DestinationAccount"].AsString,
                            FailAt = row.GetValue("LastLoginAt", 0d).AsDouble.ToNullableLocalDateTime(),
                            Manager = row.GetValue("Manager", BsonValue.Create("")).AsString,
                            Payway = ((Payway)row["TransferTransactionInfo"]["Target"]["Payway"].AsInt32).ToString("F"),
                            TransactionNo = row["FiTransactionNo"].AsString
                        };
                    });
                }
            }
            return result;
        }
    }
}
