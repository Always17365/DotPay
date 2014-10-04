using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Tools.SmsInterface
{
    public class SmsHelper
    {
        static ISms sms;
        static ISms backup_sms;

        public static void SetSmsInterface(SmsInterfaceType interfaceType, string account, string password)
        {
            switch (interfaceType)
            {
                case SmsInterfaceType.IHUYI:
                    sms = IHUYISms.Initialize(account, password);
                    break;
                case SmsInterfaceType.C123:
                    sms = C123Sms.Initialize(account, password);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetBackupSmsInterface(SmsInterfaceType interfaceType, string account, string password)
        { 
            switch (interfaceType)
            {
                case SmsInterfaceType.IHUYI:
                    backup_sms = IHUYISms.Initialize(account, password);
                    break;
                case SmsInterfaceType.C123:
                    backup_sms = C123Sms.Initialize(account, password);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Send(string mobile, string content)
        {
            Task.Factory.StartNew(() =>
            {
                sms.Send(mobile, content);
            }).Wait();
        }
        public static void SendByBackup(string mobile, string content)
        {
            Task.Factory.StartNew(() =>
            {
                backup_sms.Send(mobile, content);
            }).Wait();
        }

        public static int Balance()
        {
            return sms.Balance();
        }
    }
}
