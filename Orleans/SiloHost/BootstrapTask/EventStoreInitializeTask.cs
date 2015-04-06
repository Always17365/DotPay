using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orleans.EventSourcing;

namespace Dotpay.SiloHost.BootstrapTask
{
    internal static class EventStoreInitializeTask
    {
        internal static void Run()
        {
            var eventStoreSection = (EventStoreSection)ConfigurationManager.GetSection("eventStoreProvider");
            EventStoreProviderManager.Initailize(eventStoreSection);

            var assembly = Assembly.LoadFrom(".\\Applications\\Dotpay.Actor.Implementations\\Dotpay.Actor.Implementations.dll");

            GrainInternalEventHandlerProvider.RegisterInternalEventHandler(assembly);
            var typeNameCodeMapping = GetEventTypeNameAndCodeMapping();
            if (typeNameCodeMapping.Any())
            {
                var types = assembly.GetTypes();
                foreach (var kv in typeNameCodeMapping)
                {
                    var type = types.Single(t => t.FullName == kv.Key);

                    EventNameTypeMapping.RegisterEventType(kv.Value, type);
                }
            }
        }

        private static Dictionary<string, uint> GetEventTypeNameAndCodeMapping()
        {
            //Generate from EventTypeCodeRegisterTool
            var typeCodeDic = new Dictionary<string, uint>();
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionInitializedEvent", 1001);
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionPreparationCompletedEvent", 1002);
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionConfirmedEvent", 1003);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerInitializedEvent", 1004);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginSuccessedEvent", 1005);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginFailedEvent", 1006);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLockedEvent", 1007);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerUnlockedEvent", 1008);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerSetMobileEvent", 1009);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerIdentityVerifiedEvent", 1010);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginPasswordChangedEvent", 1011);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginPasswordResetEvent", 1012);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerPaymentPasswordChangedEvent", 1013);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerAssignedRolesEvent", 1014);
            typeCodeDic.Add("Dotpay.Actor.Events.AccountInitializeEvent", 1015);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationAddedEvent", 1016);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationCommittedEvent", 1017);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationCanceledEvent", 1018);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionInitializedEvent", 1019);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionPreparationCompletedEvent", 1020);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionConfirmedEvent", 1021);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionFailedEvent", 1022);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferInitilizedEvent", 1023);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.ConfirmedTransferTransactionPreparationEvent", 1024);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionCanceldEvent", 1025);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionMarkedAsProcessingEvent", 1026);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionConfirmCompletedEvent", 1027);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionConfirmedFailEvent", 1028);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionSubmitedToRippleEvent", 1029);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionResubmitedToRippleEvent", 1030);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionConfirmedRipplePresubmitEvent", 1031);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionConfirmedRippleTxCompleteEvent", 1032);
            typeCodeDic.Add("Dotpay.Actor.Events.Transaction.TransferTransactionConfirmedRippleTxFailEvent", 1033);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFIInitialized", 1034);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFICompleted", 1035);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFISettingUpdated", 1036);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToDotpaySettingUpdated", 1037);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPreRegister", 1038);
            typeCodeDic.Add("Dotpay.Actor.Events.UserInitialized", 1039);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginSuccessed", 1040);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginFailed", 1041);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLocked", 1042);
            typeCodeDic.Add("Dotpay.Actor.Events.UserUnlocked", 1043);
            typeCodeDic.Add("Dotpay.Actor.Events.UserSetMobile", 1044);
            typeCodeDic.Add("Dotpay.Actor.Events.SmsCounterIncreased", 1045);
            typeCodeDic.Add("Dotpay.Actor.Events.UserIdentityVerified", 1046);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordChanged", 1047);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordForget", 1048);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordReset", 1049);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordChanged", 1050);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordForget", 1051);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordReset", 1052);
            typeCodeDic.Add("Dotpay.Actor.Events.UserAssignedRoles", 1053);


            return typeCodeDic;
        }
    }
}
