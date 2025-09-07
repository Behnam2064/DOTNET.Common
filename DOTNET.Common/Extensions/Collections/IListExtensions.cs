using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Collections
{
    public static class IListExtensions
    {
        /// <summary>
        /// If the array length is odd, the last value is ignored to avoid crashing
        /// Uri.EscapeDataString is used to avoid problems such as spaces or special characters.
        /// var pairs = new[] { "page", "2", "sort", "name", "filter", "active" };
        /// 
        /// string queryString = BuildQueryString(pairs);
        /// Result:
        /// ?page=2&sort=name&filter=active
        /// </summary>
        /// <param name="queryStringPairs">Your pairs key value array or list</param>
        /// <returns></returns>
        public static string BuildQueryString(this IList<string> queryStringPairs)
        {
            if (queryStringPairs == null || queryStringPairs.Count() < 2)
                return string.Empty;

            var queryParams = new List<string>();

            for (int i = 0; i < queryStringPairs.Count() - 1; i += 2)
            {
                string key = Uri.EscapeDataString(queryStringPairs[i]);
                string value = Uri.EscapeDataString(queryStringPairs[i + 1]);
                queryParams.Add($"{key}={value}");
            }

            return "?" + string.Join("&", queryParams);
        }

        public static void AddHttpHeaders(this IList<string> headers, HttpRequestHeaders httpHeader, bool encode = true)
        {
            if (headers == null || headers.Count < 2)
                return;

            for (int i = 0; i < headers.Count - 1; i += 2)
            {
                var key = encode ? Uri.EscapeDataString(headers[i]) : headers[i];
                var value = encode ? Uri.EscapeDataString(headers[i + 1]) : headers[i + 1];
                httpHeader.Add(key, value);
            }
        }
    }
}
