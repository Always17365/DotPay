using DotPay.Common;
using DotPay.Tools.SmsInterface;
using System;
using Xunit;

namespace DotPay.Tools.Test
{
    public class SmsInterfaceTest
    {
        public SmsInterfaceTest()
        {
            TestEnvironment.Init();
        }

        [Fact]
        public void TestIHUIYISendSms()
        {
            var account = "cf_bfwl";
            var password = "HvebKj";

            //短信发送需要费用，单元测试通过后即不再重复测试
            //SmsHelper.SetSmsInterface(SmsInterfaceType.IHUYI, account, password);

            //SmsHelper.Send("18501733225", string.Format("您的验证码是：{0}。请不要把验证码泄露给其他人。", new Random().Next(1000, 9999)));
        }

        [Fact]
        public void TestC123endSms()
        {
            var account = "500919170001";
            var password = "fc_sms_account!1";

            ////短信发送需要费用，单元测试通过后即不再重复测试
            //SmsHelper.SetSmsInterface(SmsInterfaceType.C123, account, password);

            //SmsHelper.Send("18501733225", string.Format("您的验证码是：{0}。请不要把验证码泄露给其他人。", new Random().Next(1000, 9999)));
        }


        [Fact]
        public void TestSmsInterfaceBalance()
        {
            var account = "cf_bfwl";
            var password = "HvebKj";

            SmsHelper.SetSmsInterface(SmsInterfaceType.IHUYI, account, password);

            Assert.True(SmsHelper.Balance() > 0);
        }
    }
}
