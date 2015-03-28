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

        #region MQ Name

        public const string ExechangeSuffix = "Exchange";
        public const string QueueSuffix = "Queue";
        public const string DepositTransactionManagerMQName = "DepositTransactionManager";

        public const string TransferTransactionManagerMQName = "TransferTransactionManager";
        public const string TransferToRippleQueueName = "TransferToRipple";
        public const string TransferTransactionManagerRouteKey = "Inside";
        public const string TransferToRippleRouteKey = "ToRipple";

        public const string RefundTransactionManagerMQName = "RefundTransactionManager";

        public const string RippleTxResultMQName = "RippleTxResult";
        public const string RippleValidatorMQExchangeName = "RippleValidator";
        public const string RippleLedgerIndexResultQueueName = "RippleLedgerIndex";
        public const string RippleValidateResultQueueName = "RippleValidator";

        public const string RippleToFinancialInstitutionMQName = "RippleToFinancialInstitution";

        public const string RippleTrustLineMQName = "__RippleTrustLine_";


        public const string UserMQName = "UserMQ";

        #endregion
    }
}
