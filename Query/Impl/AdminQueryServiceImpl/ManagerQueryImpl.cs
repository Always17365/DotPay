using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminQueryService;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dotpay.AdminQueryServiceImpl
{
    [QueryService]
    public class ManagerQueryImpl : IManagerQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.Manager";

        public async Task<Guid?> GetManagerIdByLoginName(string loginName)
        {
            Guid? result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("LoginName", loginName);
            var projection = BsonDocument.Parse("{Id:1,_id:0}");
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
                    var item = results.First();
                    result = new Guid(item["Id"].AsString);
                }
            }
            return result;
        }

        public async Task<string> GetManagerTwofactorKeyById(Guid managerId)
        {
            string result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("Id", managerId.ToString());
            var projection = BsonDocument.Parse("{TwofactorKey:1,_id:0}");
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
                    var item = results.First();
                    result = item["TwofactorKey"].AsString;
                }
            }
            return result;
        }

        public async Task<ManagerIdentity> GetManagerIdentityById(Guid managerId)
        {
            ManagerIdentity result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("Id", managerId.ToString());
            var projection = BsonDocument.Parse("{LoginName:1,Roles:1,_id:0}");
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
                    var item = results.First();
                    var json = results.First().ToJson();
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<ManagerIdentity>(json);
                    result.ManagerId = managerId;
                }
            }
            return result;
        }

        public async Task<int> CountManagerBySearch(string loginName)
        {
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = BsonDocument.Parse(string.IsNullOrEmpty(loginName) ? "{}" : "{ LoginName : { $regex : '" + loginName + "', $options : 'i' } }");

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<ManagerListViewModel>> GetManagerBySearch(string loginName, int start, int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            IEnumerable<ManagerListViewModel> result = null;
            var filter = BsonDocument.Parse(string.IsNullOrEmpty(loginName) ? "{}" : "{ LoginName : { $regex : '" + loginName + "', $options : 'i' } }");
            var projection = BsonDocument.Parse("{Id:1,LoginName:1,IsLocked:1,LockedAt:1,LastLoginIp:1,Reason:1,LastLoginAt:1,Roles:1,_id:0}");
            var sort = BsonDocument.Parse("{LoginName:1}");
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
                    var json = results.ToJson();
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<IEnumerable<ManagerListViewModel>>(json);
                }
            }
            return result;
        }
    }
}
