using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dotpay.FrontQueryServiceImpl
{
    [QueryService]
    public class UserQuery : IUserQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.User";

        public async Task<UserIdentity> GetUserByEmail(string email)
        {
            UserIdentity result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("Email", email.ToLower());
            var projection = BsonDocument.Parse("{Id:1,LoginName:1,Email:1,_id:0}");
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
                    result = new UserIdentity
                    {
                        UserId = new Guid(item["Id"].AsString),
                        Email = item["Email"].AsString,
                        LoginName = item["LoginName"].AsString
                    };
                }
            }
            return result;
        }

        public async Task<UserIdentity> GetUserByLoginName(string loginName)
        {
            UserIdentity result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("LoginName", loginName.ToLower());
            var projection = BsonDocument.Parse("{Id:1,LoginName:1,Email:1,_id:0}");
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
                    result = new UserIdentity
                    {
                        UserId = new Guid(item["Id"].AsString),
                        Email = item["Email"].AsString,
                        LoginName = item["LoginName"].AsString
                    };
                }
            }
            return result;
        }
    }
}
