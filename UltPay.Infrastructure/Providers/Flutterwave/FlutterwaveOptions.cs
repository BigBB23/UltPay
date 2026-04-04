using System;
using System.Collections.Generic;
using System.Text;

namespace UltPay.Infrastructure.Providers.Flutterwave;

public class FlutterwaveOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string AccessToken {  get; set; }    = string.Empty;
}
