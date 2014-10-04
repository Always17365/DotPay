using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class OpenAuthShipRepository : FC.Framework.NHibernate.Repository, IOpenAuthShipRepository
    {
        public OpenAuthShipRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public OpenAuthShip FindByOpenID(string openID, OpenAuthType openAuthType)
        {
            return this._session.QueryOver<OpenAuthShip>().Where(o => o.OpenID == openID && o.Type == openAuthType).SingleOrDefault();
        }


        public OpenAuthShip FindByUserID(int userID)
        {
            return this._session.QueryOver<OpenAuthShip>().Where(o => o.UserID == userID).SingleOrDefault();
        }
    }
}
