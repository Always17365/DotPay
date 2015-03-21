using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotpay.Common
{
    public partial class Constants
    {

        /// <summary>current user key
        /// </summary>
        public const string CurrentUserKey = "__CURRENTUSER_ID_EMAIL";
        /// <summary> time out period( Seconds )
        /// </summary>
        public const int MessageConsumerTimeoutPeriod = 20;
        /// <summary> time out period( Minutes )
        /// </summary>
        public const int AtleastOnActivationInSiloGrainSelfCheckPeriod = 2;
    }
}
