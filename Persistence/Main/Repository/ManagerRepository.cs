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
    public class ManagerRepository : FC.Framework.NHibernate.Repository, IManagerRepository
    {
        public ManagerRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public IEnumerable<Manager> FindByUserID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            var managers = this._session.QueryOver<Manager>().Where(manager => manager.UserID == userID).List();

            return managers;
        }

        public void Remove(Manager manager)
        {
            Check.Argument.IsNotNull(manager, "manager");

            this._session.Delete(manager);
        }
    }
}
