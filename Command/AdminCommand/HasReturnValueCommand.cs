using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;

namespace Dotpay.AdminCommand
{
    public class HasReturnValueCommand<T> : Command
    {
        public T CommandResult { get; set; }
    }
}
