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
    public interface IWithdraw : IEntity
    {
        int ID { get; }
        string UniqueID { get; }
        int UserID { get; }
        int AccountID { get; }
        decimal Amount { get; }
        decimal Fee { get; }
        WithdrawState State { get; }
        void Verify(int byUserID, string memo);
        void SkipVerify();
    }

    public interface ICNYWithdraw : IWithdraw
    {
        int TransferAccountID { get; }
        int ReceiverAccountID { get; }
        void SetFee(decimal fee);
        void ModifyReceiverAccount(int receiverBankAccountID, int byUserID);
        void SubmitToProcess(int byUserID);
        void TranferFail(string memo, int byUserID);
        void Cancel(string memo, int byUserID);
        void Complete(int transferAccountID, string transferNo, int byUserID);
    } 
}
