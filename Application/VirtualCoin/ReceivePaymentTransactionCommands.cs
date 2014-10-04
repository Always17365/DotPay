using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    public class CreateReceivePaymentTransaction : FC.Framework.Command
    {
        public CreateReceivePaymentTransaction(string txid, string receiveCoinAddress, decimal amount, CurrencyType currency, int byUserID = 0)
        {
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotEmpty(receiveCoinAddress, "receiveCoinAddress");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.TxID = txid;
            this.ByUserID = byUserID;
            this.ReceiveCoinAddress = receiveCoinAddress;
            this.Amount = amount;
            this.Currency = currency;
            this.CommandFlg = ReceivePaymentTxCommandFlg.Create;
        }

        public int ByUserID { get; private set; }
        public string TxID { get; private set; }
        public string ReceiveCoinAddress { get; private set; }
        public decimal Amount { get; private set; }
        public CurrencyType Currency { get; private set; }
        public ReceivePaymentTxCommandFlg CommandFlg { get; set; }
    }

    public class ConfirmReceivePaymentTransaction : FC.Framework.Command
    {
        public ConfirmReceivePaymentTransaction(string txid, string receiveCoinAddress, int confirmations, decimal amount, CurrencyType currency, int byUserID = 0, int receivePaymentTxId = 0)
        {
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotEmpty(receiveCoinAddress, "receiveCoinAddress");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.TxID = txid;
            this.ReceiveCoinAddress = receiveCoinAddress;
            this.Confirmations = confirmations;
            this.ByUserID = byUserID;
            this.ReceivePaymentTxId = receivePaymentTxId;
            this.Amount = amount;
            this.Currency = currency;
            this.CommandFlg = ReceivePaymentTxCommandFlg.Confirm;
        }

        public string TxID { get; private set; }
        public string ReceiveCoinAddress { get; private set; }
        public int Confirmations { get; private set; }
        public int ByUserID { get; private set; }   //for stellar 
        public int ReceivePaymentTxId { get; private set; } //for stellar 
        public decimal Amount { get; private set; }
        public CurrencyType Currency { get; private set; }
        public ReceivePaymentTxCommandFlg CommandFlg { get; private set; }
    }

    public enum ReceivePaymentTxCommandFlg
    {
        None = 0,
        Create = 1,
        Confirm = 2
    }
}
