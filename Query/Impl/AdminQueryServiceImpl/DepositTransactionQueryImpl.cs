using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminQueryService;
using Dotpay.Common;
using Dotpay.Common.Enum;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dotpay.AdminQueryServiceImpl
{
    [QueryService]
    public class DepositTransactionQueryImpl : IDepositTransactionQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.DepositTransaction";


        public async Task<int> CountDepositTransactionBySearch(string email, string sequenceNo, Payway? payway, DepositStatus status)
        {
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var condation1 = string.IsNullOrEmpty(email)
              ? ""
              : " Email : { $regex : '" + email.ToLower() + "', $options : 'i' }";
            var condation2 = payway.HasValue ? "Payway:" + payway.Value.ToString("D") : "";
            var condation3 = (default(DepositStatus) != status ? "Status: " + status.ToString("D") : "");
            var condation4 = !string.IsNullOrEmpty(sequenceNo) ? "SequenceNo:'" + sequenceNo + "'" : "";

            var condations = new[] { condation1, condation2, condation3, condation4 }; 

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<DepositListViewModel>> GetDepositTransactionBySearch(string email, string sequenceNo, Payway? payway, DepositStatus status, int start, int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            IEnumerable<DepositListViewModel> result = null;
            var condation1 = string.IsNullOrEmpty(email)
              ? ""
              : " Email : { $regex : '" + email.ToLower() + "', $options : 'i' }";
            var condation2 = payway.HasValue ? "Payway:" + payway.Value.ToString("D") : "";
            var condation3 = (default(DepositStatus) != status ? "Status: " + status.ToString("D") : "");
            var condation4 = !string.IsNullOrEmpty(sequenceNo) ? "SequenceNo:'" + sequenceNo+"'" : "";

            var condations = new[] { condation1, condation2, condation3, condation4 };

            var filter = BsonDocument.Parse("{" + string.Join(",", condations.Where(c => !string.IsNullOrEmpty(c))) + " }");
            //var projection =
            //    BsonDocument.Parse(
            //        "{Id:1,LoginName:1,Email:1,IsLocked:1,CreateAt:1,ActiveAt:1,LockedAt:1,LastLoginIp:1,Reason:1,IdentityInfo:1,LastLoginAt:1,_id:0}");
            var sort = BsonDocument.Parse("{CreateAt:-1}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = pagesize,
                //Projection = projection,
                Skip = start,
                Sort = sort
            };
            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                if (results.Any())
                {
                    foreach (var item in results)
                    {
                        item.Remove("_id");
                    }

                    var json = results.ToJson();
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<IEnumerable<DepositListViewModel>>(json);

                    foreach (var item in results)
                    {
                        item.Remove("_id");
                    }
                }
            }
            return result;
        }
    }
}