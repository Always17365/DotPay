using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using FC.Framework;
using System.Threading.Tasks;

namespace DotPay.Tools.RippleClient
{
    public partial class RippleClient4Net
    {
        public async Task<SendMoneyResponse> SendMoneyAsnc(string recipient, decimal amount, string publicKey = "", int fee = 1, int deadLine = 900)
        {
            var result = default(SendMoneyResponse);

            var parameters = new Dictionary<string, string>();

            parameters.Add("secretPhrase", this.secretPhrase);
            parameters.Add("recipient", recipient);
            parameters.Add("amountNQT", (amount * 100000000).ToString());
            parameters.Add("feeNQT", (fee * 100000000).ToString());
            parameters.Add("deadline", deadLine.ToString());
            if (!string.IsNullOrEmpty(publicKey))
            {
                parameters.Add("publicKey", publicKey);
            }

            result = await this.HttpPost<SendMoneyResponse>("sendMoney", parameters);

            return result;
        }

        public async Task<GetAccountIdResponse> GetAccountIDAsync(string secretPhrase)
        {
            var result = default(GetAccountIdResponse);

            var parameters = new Dictionary<string, string>();

            parameters.Add("secretPhrase", secretPhrase);
            result = await this.HttpPost<GetAccountIdResponse>("getAccountId", parameters);

            return result;
        }

        public async Task<GetAccountTransactionIdsResponse> GetAccountTransactionIdsAsync(UInt64 accountID, int timestamp)
        {
            var result = default(GetAccountTransactionIdsResponse);

            var parameters = new Dictionary<string, string>();

            if (timestamp < this.genesis_timestamp)
            {
                timestamp = 0;
            }
            else timestamp = timestamp - this.genesis_timestamp;

            parameters.Add("account", accountID.ToString());
            parameters.Add("timestamp", timestamp.ToString());

            result = await this.HttpPost<GetAccountTransactionIdsResponse>("getAccountTransactionIds", parameters);

            return result;
        }

        public async Task<GetTransactionResponse> GetTransactionAsync(string transactionId)
        {
            var result = default(GetTransactionResponse);

            var parameters = new Dictionary<string, string>();

            parameters.Add("transaction", transactionId);
            result = await this.HttpPost<GetTransactionResponse>("getTransaction", parameters);

            return result;
        }

        public async Task<GetBalanceResponse> GetBalance(string account)
        {
            var result = default(GetBalanceResponse);

            var parameters = new Dictionary<string, string>();

            parameters.Add("account", account);
            result = await this.HttpPost<GetBalanceResponse>("getBalance", parameters);

            return result;
        }
    }
}
