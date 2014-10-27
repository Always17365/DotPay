using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class ScoreRecordMap : BaseClassMapping<ScoreRecord>
    {
        public ScoreRecordMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));
            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_SCORERECORD_USERID"); });
            Property(x => x.SourceID, map => { map.NotNullable(true); });
            Property(x => x.SourceUniqueID, map => { map.NotNullable(true); map.Length(22); });
            Property(x => x.Score, map => { map.NotNullable(true); });
            Property(x => x.Source, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); map.Index("IX_SCORERECORD_CREATE_AT"); });
            Version(x => x.Version, map => { });
        }
    }
}
