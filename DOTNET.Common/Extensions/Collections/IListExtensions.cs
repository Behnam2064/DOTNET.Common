using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Collections
{
    public static class IListExtensions
    {
        /// <summary>
        /// If the array length is odd, the last value is ignored to avoid crashing
        /// Uri.EscapeDataString is used to avoid problems such as spaces or special characters.
        /// </summary>
        /// <param name="queryStringPairs"></param>
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

    }
}
