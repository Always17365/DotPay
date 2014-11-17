using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    #region PreRegistrationCreated event
    public class PreRegistrationCreated : DomainEvent
    {
        public PreRegistrationCreated(string email, string token)
        {
            this.Email = email;
            this.Token = token;
        }
        public string Email { get; private set; }
        public string Token { get; private set; }
    }
    #endregion

    #region PreRegistration  Verify event
    public class PreRegistrationVerifyed : DomainEvent
    {
        public PreRegistrationVerifyed(int preRegistrationID, string token)
        {
            this.PreRegistrationID = preRegistrationID;
            this.Token = token;
        }
        public int PreRegistrationID { get; private set; }
        public string Token { get; private set; }
    }
    #endregion

    #region PreRegistration Refresh event
    public class PreRegistrationRefreshed : DomainEvent
    {
        public PreRegistrationRefreshed(int preRegistrationID, string email, string token)
        {
            this.PreRegistrationID = preRegistrationID;
            this.Email = email;
            this.Token = token;
        }
        public int PreRegistrationID { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
    }
    #endregion
}
