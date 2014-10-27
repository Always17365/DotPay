using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public interface IDeposit : IEntity
    {
        int ID { get; }
        string SequenceNo { get; }
        int UserID { get; }
        int AccountID { get; }
        decimal Amount { get; }
        DepositState State { get; }
    }

    public interface ICNYDeposit : IDeposit
    {
        void VerifyAmount(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount);
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
