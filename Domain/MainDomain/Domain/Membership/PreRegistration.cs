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
namespace DotPay.MainDomain
{

    public class PreRegistration : DomainBase, IAggregateRoot,
                 IEventHandler<PreRegistrationCreated>,
                 IEventHandler<PreRegistrationVerifyed>,
                 IEventHandler<PreRegistrationRefreshed>
    {
        #region ctor
        protected PreRegistration() { }

        public PreRegistration(string email)
        {
            var token = this.GenerteEmailValidateToken(email);
            this.RaiseEvent(new PreRegistrationCreated(email, token));
        }

        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string Email { get; set; }
        public virtual bool IsEmailVerify { get; protected set; }
        public virtual string EmailValidateToken { get; protected set; }
        #endregion

        #region public method
        public virtual void Refresh()
        {
            if (this.IsEmailVerify)
                throw new PreRegistrationHasVerifiedException();
            else
            {
                var token = this.GenerteEmailValidateToken(this.Email);
                this.RaiseEvent(new PreRegistrationRefreshed(this.ID, this.Email, token));
            }
        }

        public virtual void Verify(string token)
        {
            if (this.IsEmailVerify)
                throw new PreRegistrationHasVerifiedException();
            else
                this.RaiseEvent(new PreRegistrationVerifyed(this.ID, token));
        }
        #endregion

        #region Event Handlers
        void IEventHandler<PreRegistrationCreated>.Handle(PreRegistrationCreated @event)
        {
            this.EmailValidateToken = GenerteEmailValidateToken(@event.Email);
            this.Email = @event.Email;
            this.IsEmailVerify = false;
        }
        void IEventHandler<PreRegistrationVerifyed>.Handle(PreRegistrationVerifyed @event)
        {
            this.IsEmailVerify = true;
        }
        void IEventHandler<PreRegistrationRefreshed>.Handle(PreRegistrationRefreshed @event)
        {
            this.EmailValidateToken = @event.Token;
        }
        #endregion

        #region private method
        private string GenerteEmailValidateToken(string email)
        {
            return CryptoHelper.MD5(email + DateTime.Now.ToUnixTimestamp());
        }
        #endregion

    }
}
