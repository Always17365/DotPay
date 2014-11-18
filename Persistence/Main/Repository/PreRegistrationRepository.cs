using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class PreRegistrationRepository : FC.Framework.NHibernate.Repository, IPreRegistrationRepository
    {
        public PreRegistrationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public PreRegistration FindByEmail(string email)
        {
            Check.Argument.IsNotEmpty(email, "email");

            var users = this._session.QueryOver<PreRegistration>().Where(pr => pr.Email == email);

            return users.SingleOrDefault();
        }
    }
}
