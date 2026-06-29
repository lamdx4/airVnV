using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Airbnb.PaymentService.Infrastructure;

public class VnpayLibrary
{
    private readonly SortedList<string, string> requestData = new(new VnpayComparer());
    private readonly SortedList<string, string> responseData = new(new VnpayComparer());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            responseData.Add(key, value);
        }
    }

    public string GetResponseData(string key)
    {
        return responseData.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in requestData)
        {
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }

        var queryString = data.ToString();
        baseUrl += "?" + queryString;
        var rawData = data.ToString().Remove(data.Length - 1);
        var vnpSecureHash = HmacSha512(hashSecret, rawData);
        baseUrl += "vnp_SecureHash=" + vnpSecureHash;

        return baseUrl;
    }

    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var data = new StringBuilder();
        foreach (var kv in responseData)
        {
            if (!string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }

        var rawData = data.ToString().Remove(data.Length - 1);
        var checkHash = HmacSha512(secretKey, rawData);

        return checkHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSha512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }
        return hash.ToString();
    }
}

public class VnpayComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return string.CompareOrdinal(x, y);
    }
}
