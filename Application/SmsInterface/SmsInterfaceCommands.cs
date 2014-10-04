using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Sms Interface Command
    [ExecuteSync]
    public class CreateSmsInterface : FC.Framework.Command
    {
        public CreateSmsInterface(SmsInterfaceType smsType, string account, string password, int createBy)
        {
            Check.Argument.IsNotEmpty(account, "account");
            Check.Argument.IsNotEmpty(password, "password"); 
            Check.Argument.IsNotNegativeOrZero(createBy, "createBy");

            this.SmsType = smsType;
            this.Account = account;
            this.Password = password;
            this.CreateBy = createBy;
        }

        public virtual SmsInterfaceType SmsType { get; private set; }
        public virtual string Account { get; private set; }
        public virtual string Password { get; private set; }
        public virtual int CreateBy { get; private set; }
    }
    #endregion

    #region Create Sms Interface Command
    [ExecuteSync]
    public class ModifySmsInterface : FC.Framework.Command
    {
        public ModifySmsInterface(int smsInterfaceID, SmsInterfaceType smsType, string account, string password, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(smsInterfaceID, "smsInterfaceID");
            Check.Argument.IsNotEmpty(account, "account");
            Check.Argument.IsNotEmpty(password, "password");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.SmsInterfaceID = smsInterfaceID;
            this.SmsType = smsType;
            this.Account = account;
            this.Password = password;
            this.ByUserID = byUserID;
        }


        public virtual int SmsInterfaceID { get; private set; }
        public virtual SmsInterfaceType SmsType { get; private set; }
        public virtual string Account { get; private set; }
        public virtual string Password { get; private set; }
        public virtual int ByUserID { get; private set; }
    }
    #endregion
}
