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

namespace Dotpay.FrontQueryServiceImpl
{
    [QueryService]
    public class AccountQueryImpl : IAccountQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.Account";
        public async Task<List<AccountBalanceViewModel>> GetAccountBalanceByOwnerId(Guid userId)
        {
            var result = new List<AccountBalanceViewModel>();
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("OwnerId", userId.ToString());
            var projection = BsonDocument.Parse("{Id:1,Balances:1,_id:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1,
                Projection = projection
            };

            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                if (results.Any())
                {
                    var row = results.FirstOrDefault();
                    var balances = row["Balances"].ToJson();

                    result = IoC.Resolve<IJsonSerializer>().Deserialize<List<AccountBalanceViewModel>>(balances);
                }
            }
            return result;
        }

        public async Task<Guid> GetAccountIdByOwnerId(Guid userId)
        {
            Guid result;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("OwnerId", userId.ToString());
            var projection = BsonDocument.Parse("{Id:1,OwnerId:1,_id:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1,
                Projection = projection
            };

            using (var cursor = await collection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                var row = results.FirstOrDefault();
                result =row["OwnerId"].AsGuid;
            }
            return result;
        }
    }
}
