using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class BaseClassMapping<T> : ClassMapping<T> where T : class
    {
        public BaseClassMapping()
        {
            Table(Config.Table_Prefix + typeof(T).Name);
        }
    }
}
