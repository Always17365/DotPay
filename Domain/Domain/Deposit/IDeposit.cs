using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public interface IDeposit : IEntity
    {
        int ID { get; }
        string UniqueID { get; }
        int UserID { get; }
        int AccountID { get; }
        decimal Amount { get; }
        decimal Fee { get; }
        DepositState State { get; }
    }

    public interface ICNYDeposit : IDeposit
    {
        int FundSourceID { get; }
        string FundExtra { get; }
        void SetAmountAndFee(decimal amount, decimal fee);
        void VerifyAmount(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount, string fundExtra);
        void Complete(int byUserID);
        void UndoComplete(int byUserID);
    }

    public interface IVirtualCoinDeposit : IDeposit
    {
        string TxID { get; }
        void VerifyAmount(int byUserID, string txid, decimal txAmount);
        void Complete(CurrencyType currencyType, int byUserID);
    }
}
