using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Tools.SmsInterface
{
    public interface ISms
    {
        void Send(string mobile, string content);

        int Balance();
    }
}
