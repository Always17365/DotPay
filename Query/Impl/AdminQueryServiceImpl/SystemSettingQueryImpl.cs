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
    public class SystemSettingQueryImpl : ISystemSettingQuery
    {
        private const string CollectionName = "Dotpay.Actor.Implementations.SystemSetting";

        //public async Task<SystemSettingViewModel> GetSystemSetting()
        //{
        //    SystemSettingViewModel result = null;
        //    var settingCollection = MongoManager.GetCollection<BsonDocument>(CollectionName);

        //    var filter = BsonDocument.Parse("{}");
        //    var options = new FindOptions<BsonDocument, BsonDocument>
        //    {
        //        Limit = 1
        //    };

        //    using (var cursor = await settingCollection.FindAsync<BsonDocument>(filter, options))
        //    {
        //        var results = await cursor.ToListAsync();

        //        if (results.Any())
        //        {
        //            var settingJson = results.First().ToJson();
        //            result = IoC.Resolve<IJsonSerializer>().Deserialize<SystemSettingViewModel>(settingJson);
        //        }
        //    }
        //    return result;
        //}

        public async Task<ToFISettingViewModel> GetToFISetting()
        {
            SystemSettingViewModel result = null;
            var settingCollection = MongoManager.GetCollection<BsonDocument>(CollectionName);

            var filter = BsonDocument.Parse("{}");
            var projection = BsonDocument.Parse("{RippleToFinancialInstitutionSetting:1,_id:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1,
                Projection = projection
            };
            using (var cursor = await settingCollection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();

                if (results.Any())
                {
                    var item = results.First(); 
                    var settingJson = item.ToJson();
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<SystemSettingViewModel>(settingJson);
                }
            }
            return result==null?null:result.RippleToFinancialInstitutionSetting;
        }

        public async Task<ToDotpaySettingViewModel> GetToDotpaySetting()
        {
            SystemSettingViewModel result = null;
            var settingCollection = MongoManager.GetCollection<BsonDocument>(CollectionName);

            var filter = BsonDocument.Parse("{}");
            var projection = BsonDocument.Parse("{RippleToDotpaySetting:1,_id:0}");
            var options = new FindOptions<BsonDocument, BsonDocument>
            {
                Limit = 1,
                Projection = projection
            };

            using (var cursor = await settingCollection.FindAsync<BsonDocument>(filter, options))
            {
                var results = await cursor.ToListAsync();
                if (results.Any())
                {
                    var settingJson = results.First().ToJson();
                    result = IoC.Resolve<IJsonSerializer>().Deserialize<SystemSettingViewModel>(settingJson);
                }
            }

            return result == null ? null : result.RippleToDotpaySetting;
        }
    }
}
