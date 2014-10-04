using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Repository
{
    public interface IManagerRepository : IRepository
    {
        IEnumerable<Manager> FindByUserID(int userID);

        void Remove(Manager manager);
    }
}
