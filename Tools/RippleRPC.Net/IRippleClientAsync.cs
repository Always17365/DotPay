using RippleRPC.Net.Exceptions;
using RippleRPC.Net.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace RippleRPC.Net
{
    public interface IRippleClientAsync
    {
        bool AutoReconnect { get; set; }
        System.Net.NetworkCredential Credentials { get; set; }
        Task<Tuple<RippleError, AccountInformation>> GetAccountInformation(string account, uint? index = null, string ledgerHash = null, bool strict = false, string ledgerIndex = "current");
        Task<Tuple<RippleError, List<AccountLine>>> GetAccountLines(string account, string peer = null, string ledgerIndex = "current");
        Task<Tuple<RippleError, List<AccountOffer>>> GetAccountOffers(string account, int accountIndex = 0, string ledgerHash = null, string ledgerIndex = "current");
        Task<Tuple<RippleError, List<BookOffer>>> GetBookOffers(RippleCurrency takerPays, RippleCurrency takerGets, string ledger = "current", string taker = null, int limit = 200, bool proof = false, bool autoBridge = false, Marker marker = null);
        Task<Tuple<RippleError, string>> GetClosedLedgerHash();
        Task<Tuple<RippleError, int>> GetClosedLedgerIndex();
        Task<Tuple<RippleError, int>> GetCurrentLedgerIndex();
        Task<Tuple<RippleError, List<TransactionRecord>>> GetTransactions(string account, int ledgerIndex, bool binary = false, bool forward = false, int limit = 200, Marker marker = null);
        Task<Tuple<RippleError, List<TransactionRecord>>> GetTransactions(string account, int minimumLedgerIndex = -1, int maximumLedgerIndex = -1, bool binary = false, bool forward = false, int limit = 200, Marker marker = null);
        Task<Tuple<RippleError, RippleRPC.Net.Model.Paths.PathSummary>> RipplePathFind(string fromAccount, string toAccount, RippleCurrencyValue destinationamount);
        Task<Tuple<RippleError, PaymentSubmitResult>> SendXRP(string fromAccount, string secret, string toAccount, decimal amount);
        Task<Tuple<RippleError, TransactionSigned>> Sign(Transaction transaction, string secret, bool offline = false);
        Task<Tuple<RippleError, PaymentSubmitResult>> Submit(string transactionHash);
        int TimeOut { get; set; }
        Uri Uri { get; set; }
        bool WebSocketConnected { get; }
    }
}
