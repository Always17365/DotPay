using DotPay.RippleDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.RipplePersistence.Mapper
{
    public class BaseClassMapping<T> : ClassMapping<T> where T : class
    {
        public BaseClassMapping()
        {
            Table(Config.Table_Prefix + typeof(T).Name);
        }
    }
}
