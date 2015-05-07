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
        public async Task<int> CountPendingDotpayToFiTransferTx(string email)
        {
            var condation = "{" +
                            (string.IsNullOrEmpty(email) ? "" : "Email : { $regex : '" + email + "', $options : 'i' },") +
                                  "Status:{ $in:[" + TransferTransactionStatus.PreparationCompleted.ToString("D") +
                                       "," + TransferTransactionStatus.LockeByProcessor.ToString("D") + "]}," +
                                  "'TransactionInfo.Target.Payway':{ $ne:" + Payway.Dotpay.ToString("D") + "}}";
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = BsonDocument.Parse(condation);

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetPendingDotpayToFiTransferTx(string email, int start, int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            List<TransferFromDotpayToFiListViewModel> result = null;
            var condation = "{" +
                         (string.IsNullOrEmpty(email) ? "" : "Email : { $regex : '" + email + "', $options : 'i' },") +
                               "Status:{ $in:[" + TransferTransactionStatus.PreparationCompleted.ToString("D") +
                                    "," + TransferTransactionStatus.LockeByProcessor.ToString("D") + "]}," +
                               "'TransactionInfo.Target.Payway':{ $ne:" + Payway.Dotpay.ToString("D") + "}}";

            var filter = BsonDocument.Parse(condation);

            var projection = BsonDocument.Parse("{Id:1,SequenceNo:1,TransactionInfo:1,ManagerId:1,Manager:1,CreateAt:1,Memo:1,CompleteAt:1,FailAt:1,Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:-1}");
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
                        var txinfoBson = r["TransactionInfo"].AsBsonDocument;
                        var txinfoString = txinfoBson["Target"].AsBsonDocument;
                        var bank = new Bank?((Bank)txinfoString.GetValue("Bank", 0).AsInt32);
                        var item = new TransferFromDotpayToFiListViewModel()
                        {
                            Id = Guid.Parse(r["Id"].AsString),
                            Email = r["TransactionInfo"]["Source"].AsBsonDocument.GetValue("Email", "").AsString,
                            SequenceNo = r["SequenceNo"].AsString,
                            Currency = ((CurrencyType)r["TransactionInfo"]["Currency"].ToInt32()).ToString("F"),
                            Amount = (decimal)r["TransactionInfo"]["Amount"].ToDouble(),
                            Bank = bank,
                            CreateAt = r["CreateAt"].AsDouble.ToLocalDateTime(),
                            Destination = r["TransactionInfo"]["Target"]["Destination"].AsString,
                            Manager = r.GetValue("Manager", "").AsString,
                            Memo = txinfoBson.GetValue("Memo", "").AsString,
                            Payway = ((Payway)r["TransactionInfo"]["Target"]["Payway"].AsInt32).ToString("F")
                        };
                        result.Add(item);
                    });
                }
            }
            return result;
        }

        public async Task<int> CountDotpayToFiTransferTx(string email, string sequenceNo, string transferNo, TransferTransactionStatus status)
        {
            var condation1 = string.IsNullOrEmpty(email) ? "" : "Email : { $regex : '" + email + "', $options : 'i' }";
            var condation2 = string.IsNullOrEmpty(sequenceNo) ? "" : " SequenceNo:'" + sequenceNo + "'";
            var condation3 = string.IsNullOrEmpty(transferNo) ? "" : " FiTransactionNo:'" + transferNo + "'";
            var condation4 = " Status:" + status.ToString("D");
            var condation5 = "'TransactionInfo.Target.Payway':{ $ne:" + Payway.Dotpay.ToString("D") + "}";

            var condations = new[] { condation1, condation2, condation3, condation4, condation5 };

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");

            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetDotpayToFiTransferTx(
            string email, string sequenceNo, string transferNo, TransferTransactionStatus status, int start,
            int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            List<TransferFromDotpayToFiListViewModel> result = null;
            var condation1 = string.IsNullOrEmpty(email) ? "" : "Email : { $regex : '" + email + "', $options : 'i' }";
            var condation2 = string.IsNullOrEmpty(sequenceNo) ? "" : " SequenceNo:'" + sequenceNo + "'";
            var condation3 = string.IsNullOrEmpty(transferNo) ? "" : " FiTransactionNo:'" + transferNo + "'";
            var condation4 = " Status:" + status.ToString("D");
            var condation5 = "'TransactionInfo.Target.Payway':{ $ne:" + Payway.Dotpay.ToString("D") + "}";

            var condations = new[] { condation1, condation2, condation3, condation4, condation5 };

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");

            var projection =
                BsonDocument.Parse(
                    "{Id:1,SequenceNo:1,TransactionInfo:1,FiTransactionNo:1,ManagerId:1,Manager:1,CreateAt:1,CompleteAt:1,FailAt:1,Reason:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:-1}");
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
                        var txinfoBson = row["TransactionInfo"].AsBsonDocument;
                        var txinfoString = txinfoBson["Target"].AsBsonDocument;
                        var bank = new Bank?((Bank)txinfoString.GetValue("Bank", 0).AsInt32);

                        var item = new TransferFromDotpayToFiListViewModel()
                        {
                            Id = Guid.Parse(row["Id"].AsString),
                            Email = row["TransactionInfo"]["Source"].AsBsonDocument.GetValue("Email", BsonValue.Create("")).AsString,
                            SequenceNo = row["SequenceNo"].AsString,
                            Currency = ((CurrencyType)row["TransactionInfo"]["Currency"].ToInt32()).ToString("F"),
                            Amount = (decimal)row["TransactionInfo"]["Amount"].AsDouble,
                            Bank = bank,
                            CreateAt = row.GetValue("CreateAt", 0d).AsDouble.ToLocalDateTime(),
                            CompleteAt = row.GetValue("CompleteAt", 0d).AsDouble.ToLocalDateTime(),
                            Destination = row["TransactionInfo"]["Target"]["Destination"].AsString,
                            FailAt = row.GetValue("LastLoginAt", 0d).AsDouble.ToNullableLocalDateTime(),
                            Manager = row.GetValue("Manager", BsonValue.Create("")).AsString,
                            Memo = txinfoBson.GetValue("Memo", "").AsString,
                            Reason = row.GetValue("Reason", "").AsString,
                            Payway = ((Payway)row["TransactionInfo"]["Target"]["Payway"].AsInt32).ToString("F")
                        };
                        result.Add(item);
                    });
                }
            }
            return result;
        }
    }
}
