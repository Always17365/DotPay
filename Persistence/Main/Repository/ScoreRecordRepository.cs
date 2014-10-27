using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class ScoreRecordRepository : FC.Framework.NHibernate.Repository, IScoreRecordRepository
    {
        public ScoreRecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public ScoreRecord FindBySourceIDAndSourceType(int sourceID, ScoreSource source)
        {
            return this._session.QueryOver<ScoreRecord>()
                                .Where(sr => sr.SourceID == sourceID && sr.Source == source)
                                .SingleOrDefault();
        }


        public void Remove(ScoreRecord scoreRecord)
        {
            this._session.Delete(scoreRecord);
        }
    }
}
