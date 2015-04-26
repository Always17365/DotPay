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
    public class UserQueryImpl : IUserQuery
    {
        private const string COLLECTION_NAME = "Dotpay.Actor.Implementations.User";

        public async Task<UserIdentity> GetUserByEmail(string email)
        {
            var result = new List<UserIdentity>();
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("Email", email.ToLower());
            var projection = BsonDocument.Parse("{Id:1,LoginName:1,IsVerified:1,LastLoginAt:1,IdentityInfo:1,Email:1,_id:0}");
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
                    results.ForEach(row =>
                    {
                        var identityInfoStr = row.GetValue("IdentityInfo", BsonValue.Create("")).AsString;
                        var item = new UserIdentity
                        {
                            UserId = new Guid(row["Id"].AsString),
                            Email = row["Email"].AsString,
                            LoginName = row.GetValue("LoginName", BsonValue.Create("")).AsString,
                            LastLoginAt = row.GetValue("LastLoginAt", BsonValue.Create(0d)).AsDouble.ToNullableLocalDateTime(),
                            IsActive = row["IsVerified"].AsBoolean,
                            IdentityInfo = string.IsNullOrEmpty(identityInfoStr) ? null : IoC.Resolve<IJsonSerializer>().Deserialize<IdentityInfo>(identityInfoStr)
                        };
                        result.Add(item);
                    });
                }
            }
            return result.FirstOrDefault();
        }

        public async Task<UserIdentity> GetUserByLoginName(string loginName)
        {
            UserIdentity result = null;
            var collection = MongoManager.GetCollection<BsonDocument>(COLLECTION_NAME);

            var filter = new BsonDocument("LoginName", loginName.ToLower());
            var projection = BsonDocument.Parse("{Id:1,LoginName:1,IsVerified:1,LastLoginAt:1,IdentityInfo:1,Email:1,_id:0}");
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
                    var row = results.First();
                    var identityInfoStr = row.GetValue("IdentityInfo", "").AsString;
                    result = new UserIdentity
                    {
                        UserId = new Guid(row["Id"].AsString),
                        Email = row["Email"].AsString,
                        LoginName = row.GetValue("LoginName", "").AsString,
                        LastLoginAt = row.GetValue("LastLoginAt", 0d).AsDouble.ToNullableLocalDateTime(),
                        IsActive = row["IsVerified"].AsBoolean,
                        IdentityInfo = string.IsNullOrEmpty(identityInfoStr) ? null : IoC.Resolve<IJsonSerializer>().Deserialize<IdentityInfo>(identityInfoStr)
                    };
                }
            }
            return result;
        }
    }
}
