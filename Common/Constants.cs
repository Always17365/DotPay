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

        public const string TransferTransactionManagerQueueName = "TransferTransactionManagerInside";
        public const string TransferTransactionManagerToRippleQueueName = "TransferTransactionManagerToRipple";
        public const string TransferTransactionManagerRouteKey = "Inside";
        public const string TransferTransactionManagerToRippleRouteKey = "ToRipple";
        public const string RefundTransactionManagerMQName = "RefundTransactionManager";
        public const string RippleToFinancialInstitutionMQName = "RippleToFinancialInstitution";

        #endregion
    }
}
