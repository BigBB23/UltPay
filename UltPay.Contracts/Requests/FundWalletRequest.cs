using System;
using System.Collections.Generic;
using System.Text;
using UltPay.Contracts.Requests;

namespace UltPay.Contracts.Requests
{
    public class FundWalletRequest
    {
        public Guid UserId {  get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
