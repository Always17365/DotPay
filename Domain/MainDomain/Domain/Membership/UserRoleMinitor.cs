using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Repository;
using FC.Framework.Repository;
namespace DotPay.MainDomain
{
    public class UserRoleMinitor : IEventHandler<UserAssignedRole>,
                                   IEventHandler<UserUnsignedRole>
    {
        public void Handle(UserAssignedRole @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var managerRepos = IoC.Resolve<IManagerRepository>();

            if (@event.ManagerType == ManagerType.SuperManager)
                throw new ModifyUserRoleToSuperManagerException();

            var existManagers = managerRepos.FindByUserID(@event.UserID);
            var exist = false;

            foreach (var role in existManagers)
            {
                if (role.Type == @event.ManagerType)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                var manager = new Manager(@event.UserID, @event.ManagerType);
                repos.Add(manager);
            }
        }

        public void Handle(UserUnsignedRole @event)
        {
            var managerRepos = IoC.Resolve<IManagerRepository>();

            var managers = managerRepos.FindByUserID(@event.UserID);

            foreach (Manager role in managers)
            {
                if (role.Type == @event.ManagerType)
                {
                    managerRepos.Remove(role);
                    break;
                }
            }
        }
    }
}
