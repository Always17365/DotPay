using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;

namespace DotPay.RippleDomain
{
    public class DomainBase : IEntity
    {               
        public virtual int Version { get; protected set; }
    }
}
