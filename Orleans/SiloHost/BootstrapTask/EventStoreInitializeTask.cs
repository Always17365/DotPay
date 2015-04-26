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

        private static Dictionary<string, int> GetEventTypeNameAndCodeMapping()
        {
            //Generate from EventTypeCodeRegisterTool
            var typeCodeDic = new Dictionary<string, int>();
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionInitializedEvent", 1001);
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionPreparationCompletedEvent", 1002);
            typeCodeDic.Add("Dotpay.Actor.Events.RefundTransactionConfirmedEvent", 1003);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerInitializedEvent", 1004);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginSuccessedEvent", 1005);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginFailedEvent", 1006);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLockedEvent", 1007);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerUnlockedEvent", 1008);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginPasswordChangedEvent", 1009);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerLoginPasswordResetEvent", 1010);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerTwofactorKeyResetEvent", 1011);
            typeCodeDic.Add("Dotpay.Actor.Events.ManagerAssignedRolesEvent", 1012);
            typeCodeDic.Add("Dotpay.Actor.Events.AccountInitializeEvent", 1013);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationAddedEvent", 1014);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationCommittedEvent", 1015);
            typeCodeDic.Add("Dotpay.Actor.Events.TransactionPreparationCanceledEvent", 1016);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionInitializedEvent", 1017);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionPreparationCompletedEvent", 1018);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionConfirmedEvent", 1019);
            typeCodeDic.Add("Dotpay.Actor.Events.DepositTransactionFailedEvent", 1020);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferInitilizedEvent", 1021);
            typeCodeDic.Add("Dotpay.Actor.Events.ConfirmedTransferTransactionPreparationEvent", 1022);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionCanceldEvent", 1023);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionMarkedAsProcessingEvent", 1024);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionConfirmCompletedEvent", 1025);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionConfirmedFailEvent", 1026);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionSubmitedToRippleEvent", 1027);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionResubmitedToRippleEvent", 1028);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionConfirmedRipplePresubmitEvent", 1029);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionConfirmedRippleTxCompleteEvent", 1030);
            typeCodeDic.Add("Dotpay.Actor.Events.TransferTransactionConfirmedRippleTxFailEvent", 1031);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFIInitializedEvent", 1032);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFILockedEvent", 1033);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFICompletedEvent", 1034);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFIFailedEvent", 1035);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToFISettingUpdatedEvent", 1036);
            typeCodeDic.Add("Dotpay.Actor.Events.RippleToDotpaySettingUpdatedEvent", 1037);
            typeCodeDic.Add("Dotpay.Actor.Events.UserRegisterEvent", 1038);
            typeCodeDic.Add("Dotpay.Actor.Events.UserActiveTokenResetEvent", 1039);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordInitalizedEvent", 1040);
            typeCodeDic.Add("Dotpay.Actor.Events.UserActivedEvent", 1041);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginNameSetEvent", 1042);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginSuccessedEvent", 1043);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginFailedEvent", 1044);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLockedEvent", 1045);
            typeCodeDic.Add("Dotpay.Actor.Events.UserUnlockedEvent", 1046);
            typeCodeDic.Add("Dotpay.Actor.Events.UserSetMobileEvent", 1047);
            typeCodeDic.Add("Dotpay.Actor.Events.SmsCounterIncreasedEvent", 1048);
            typeCodeDic.Add("Dotpay.Actor.Events.UserIdentityVerifiedEvent", 1049);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordModifiedEvent", 1050);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordForgetEvent", 1051);
            typeCodeDic.Add("Dotpay.Actor.Events.UserLoginPasswordResetEvent", 1052);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordModifiedEvent", 1053);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordForgetEvent", 1054);
            typeCodeDic.Add("Dotpay.Actor.Events.UserPaymentPasswordResetEvent", 1055);


            return typeCodeDic;
        }
    }
}
