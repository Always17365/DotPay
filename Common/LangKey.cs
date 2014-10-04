using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// system config key , you can use key get config value
    /// </summary>
    public partial struct LangKey
    {
        #region domain lang key
        public struct DomainEmail
        {
            public const string OUR_TEAM_NAME = "Our_Team_Name";
            public const string WELCOME_REGISTER_EMAIL_TITLE = "Welcome_Register_Email_Title";
            public const string WELCOME_REGISTER_EMAIL_BODY = "Welcome_Register_Email_Body";
            public const string USER_RESET_PASSWORD_EMAIL_TITLE = "User_Reset_Password_Email_Title";
            public const string USER_RESET_PASSWORD_EMAIL_BODY = "User_Reset_Password_Email_Body";
        }
        #endregion
         
    }
}
