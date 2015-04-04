using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;

namespace Dotpay.Admin.ViewModel
{
    public class ManagerIdentity
    {
        public Guid ManagerId { get; set; }
        public string LoginName { get; set; }
        public IEnumerable<ManagerType> Roles { get; set; }
    }
}
