using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.RippleDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using System.Web;
using FC.Framework.Repository;

namespace DotPay.RippleDomain
{
    public class RippleTransferMonitor : IEventHandler<RippleInboundTxCreated>,
                                         IEventHandler<RippleInboundTxToThirdPartyPaymentCompelted>
    {
        public void Handle(RippleInboundTxCreated @event)
        {
            //当接收到ripple转账时，发送消息给dotpay处理充值
            throw new NotImplementedException();
        }

        public void Handle(RippleInboundTxToThirdPartyPaymentCompelted @event)
        {
            //当接收到ripple转账时，发送消息给dotpay处理，创建第三方支付转账数据，之后等待人工处理
            throw new NotImplementedException();
        }
    }
}
