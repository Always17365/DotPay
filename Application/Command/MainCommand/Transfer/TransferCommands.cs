using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region 内部转账
    public class CreateInsideTransfer : FC.Framework.Command
    {
        public CreateInsideTransfer(int fromUserID, int toUserID, CurrencyType currency, decimal amount, string description)
        {
            Check.Argument.IsNotNegativeOrZero(fromUserID, "fromUserID");
            Check.Argument.IsNotNegativeOrZero(toUserID, "toUserID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.FromUserID = fromUserID;
            this.ToUserID = toUserID;
            this.Currency = currency;
            this.Amount = amount;
            this.Description = description;
        }

        public int FromUserID { get; private set; }
        public int ToUserID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        /// <summary>
        /// 订单的sequence no
        /// </summary>
        public string Result { get; set; }
    }

    public class SubmitInsideTransfer : FC.Framework.Command
    {
        public SubmitInsideTransfer(string insideTransferSeq, string tradePassword, CurrencyType currency)
        {
            Check.Argument.IsNotEmpty(insideTransferSeq, "insideTransferSeq");
            Check.Argument.IsNotEmpty(tradePassword, "tradePassword");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.InsideTransferSeq = insideTransferSeq;
            this.TradePassword = tradePassword;
            this.Currency = currency;
        }

        public string InsideTransferSeq { get; private set; }
        public string TradePassword { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    #endregion

    #region 第三方支付直转

    #region 转账创建
    public class CreateThirdPartyPaymentTransfer : FC.Framework.Command
    {
        public CreateThirdPartyPaymentTransfer(string txid, string account, decimal amount, PayWay payway, PayWay sourcePayway)
        {
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotEmpty(account, "account");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");

            this.TxId = txid;
            this.Account = account;
            this.Amount = amount;
            this.PayWay = payway;
            this.SourcePayway = sourcePayway;
        }

        public string TxId { get; private set; }
        public string Account { get; private set; }
        public decimal Amount { get; private set; }
        public PayWay PayWay { get; private set; }
        public PayWay SourcePayway { get; private set; }
    }
    #endregion

    #region 转账标记处理中
    public class MarkThirdPartyPaymentTransferProcessing : FC.Framework.Command
    {
        public MarkThirdPartyPaymentTransferProcessing(int tppTransferId, PayWay payway, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(tppTransferId, "tppTransferId");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.TppTransferID = tppTransferId;
            this.PayWay = payway;
            this.ByUserID = byUserID;
        }

        public int TppTransferID { get; private set; } 
        public PayWay PayWay { get; private set; }
        public int ByUserID { get; private set; } 
    }
    #endregion

    #region 转账完成
    public class ThirdPartyPaymentTransferComplete : FC.Framework.Command
    {
        public ThirdPartyPaymentTransferComplete(int transferId, string transferNo, decimal amount, PayWay payway, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(transferId, "transferId");
            Check.Argument.IsNotEmpty(transferNo, "transferNo");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.TransferId = transferId;
            this.TransferNo = transferNo;
            this.Amount = amount;
            this.PayWay = payway;
            this.ByUserID = byUserID;
        }

        public int TransferId { get; private set; }
        public PayWay PayWay { get; private set; }
        public decimal Amount { get; private set; }
        public string TransferNo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region 转账失败
    public class ThirdPartyPaymentTransferFail : FC.Framework.Command
    {
        public ThirdPartyPaymentTransferFail(int transferId, string reason, PayWay payway, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(transferId, "transferId");
            Check.Argument.IsNotEmpty(reason, "reason");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");

            this.TransferId = transferId;
            this.Reason = reason;
            this.PayWay = payway;
            this.ByUserID = byUserID;
        }

        public int TransferId { get; private set; }
        public PayWay PayWay { get; private set; }
        public string Reason { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
    #endregion

    #region 外部-转出转账
    public class CreateOutboundTransfer : FC.Framework.Command
    {
        public CreateOutboundTransfer(PayWay payway, string destination, string targetCurrency, decimal sourceAmount, decimal targetAmount, string description, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");
            Check.Argument.IsNotEmpty(targetCurrency, "targetCurrency");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");
            Check.Argument.IsNotEmpty(destination, "destination");
            Check.Argument.IsNotNegativeOrZero(sourceAmount, "sourceAmount");
            Check.Argument.IsNotNegativeOrZero(targetAmount, "targetAmount");

            this.ByUserID = byUserID;
            this.TargetCurrency = targetCurrency;
            this.PayWay = payway;
            this.SourceAmount = sourceAmount;
            this.TargetAmount = targetAmount;
            this.Destination = destination;
            this.Description = description;
        }

        public int ByUserID { get; private set; }
        public decimal SourceAmount { get; private set; }
        public PayWay PayWay { get; private set; }
        public string TargetCurrency { get; private set; }
        public decimal TargetAmount { get; private set; }
        public string Destination { get; private set; }
        public string Description { get; private set; }
        public string Result { get; set; }
    }
    #endregion

    #region 外部-转出转账
    public class ConfirmOutboundTransfer : FC.Framework.Command
    {
        public ConfirmOutboundTransfer(string orderId, string paypassword, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");
            Check.Argument.IsNotEmpty(orderId, "orderId");
            Check.Argument.IsNotEmpty(paypassword, "paypassword");

            this.ByUserID = byUserID;
            this.SequenceNo = orderId;
            this.PayPassword = paypassword;
        }

        public int ByUserID { get; private set; }
        public string SequenceNo { get; private set; }
        public string PayPassword { get; private set; }
    }
    #endregion

}
