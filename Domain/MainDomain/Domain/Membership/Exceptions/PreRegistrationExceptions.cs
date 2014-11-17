using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Exceptions
{
    public class PreRegistrationHasVerifiedException : DomainException
    {
        public PreRegistrationHasVerifiedException() : base((int)ErrorCode.EmailHasVerified) { }
    }
 
}
