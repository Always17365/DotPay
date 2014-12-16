using RippleRPC.Net.Exceptions;
using RippleRPC.Net.Model;
using RippleRPC.Net.Model.Paths;
using System;
using System.Collections.Generic;

namespace RippleRPC.Net
{
    public interface IRippleClient
    {
        void PathFind(string fromAccount, string toAccount, RippleCurrencyValue destinationamount, Action<RippleError, PathSummary> callback = null);
        void GetAccountInformation(string account, Action<RippleError, AccountInformation> callback = null, uint? index = null, string ledgerHash = null, bool strict = false, string ledgerIndex = "current");
        void GetAccountLines(string account, Action<RippleError, System.Collections.Generic.List<AccountLine>> callback = null, string peer = null, string ledgerIndex = "current");
        void GetAccountOffers(string account, Action<RippleError, List<AccountOffer>> callback = null, int accountIndex = 0, string ledgerHash = null, string ledgerIndex = "current");
        void GetBookOffers(RippleCurrency takerPays, RippleCurrency takerGets, Action<RippleError, List<BookOffer>> callback = null, string ledger = "current", string taker = null, int limit = 200, bool proof = false, bool autoBridge = false, Marker marker = null);
        void GetClosedLedgerHash(Action<RippleError, string> callback = null);
        void GetCurrentLedgerIndex(Action<RippleError, int> callback = null);
        //LedgerSummary GetLedgerInformation(string ledgerIndex = "current", bool full = true);
        void GetTransactions(string account, Action<RippleError, List<TransactionRecord>> callback = null, int minimumLedgerIndex = -1, int maximumLedgerIndex = -1, bool binary = false, bool forward = false, int limit = 200, Marker marker = null);
        void GetTransactions(string account, int ledgerIndex, Action<RippleError, List<TransactionRecord>> callback = null, bool binary = false, bool forward = false, int limit = 200, Marker marker = null);
        void SendXRP(string fromAccount, string secret, string toAccount, decimal amount, Action<RippleError, PaymentSubmitResult> callback = null);
        void Sign(Transaction transaction, string secret, Action<RippleError, TransactionSigned> callback = null, bool offline = false);
        void Submit(string transactionHash, Action<RippleError, PaymentSubmitResult> callback = null);
        Uri Uri { get; set; }
        bool WebSocketConnected { get; }
    }
}
