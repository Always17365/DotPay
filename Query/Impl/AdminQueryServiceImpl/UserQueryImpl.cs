using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminQueryService;
using Dotpay.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dotpay.AdminQueryServiceImpl
{
    [QueryService]
    public class UserQueryImpl : IUserQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.User"; 
        public async Task<int> CountUserBySearch(string email)
        {
            long result = 0;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter =
                BsonDocument.Parse(string.IsNullOrEmpty(email)
                    ? "{}"
                    : "{ Email : { $regex : '" + email.ToLower() + "', $options : 'i' } }");

            result = await collection.CountAsync(filter);

            return (int)result;
        }

        public async Task<IEnumerable<UserListViewModel>> GetUserBySearch(string email, int start, int pagesize)
        {
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            IEnumerable<UserListViewModel> result = null;
            var filter =
                BsonDocument.Parse(string.IsNullOrEmpty(email)
                    ? "{}"
                    : "{ Email : { $regex : '" + email.ToLower() + "', $options : 'i' } }");
            var projection =
                BsonDocument.Parse(
                    "{Id:1,LoginName:1,Email:1,IsLocked:1,CreateAt:1,ActiveAt:1,LockedAt:1,LastLoginIp:1,Reason:1,IdentityInfo:1,LastLoginAt:1,_id:0}");
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
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<IEnumerable<UserListViewModel>>(json);

                    results.ForEach(u =>
                    {
                        var currentUserId = Guid.Parse(u["Id"].AsString);
                        var user = result.Single(it => it.Id == currentUserId);
                        BsonValue identityInfo;
                        user.VerifyRealName = u.TryGetValue("IdentityInfo", out identityInfo);
                    });
                }
            }
            return result;
        }

        public async Task<IdentityInfo> GetIdentityInfoById(Guid userId)
        {
            IdentityInfo result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("Id", userId);
            var projection = BsonDocument.Parse("{IdentityInfo:1,_id:0}");
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
                    result = new IdentityInfo()
                    {
                        FullName = item["IdentityInfo"]["FullName"].AsString,
                        IdNo = item["IdentityInfo"]["IdNo"].AsString,
                        IdType = (IdNoType)(item["IdentityInfo"]["IdType"].ToInt32())
                    };
                }
            }
            return result;
        }
    }
}