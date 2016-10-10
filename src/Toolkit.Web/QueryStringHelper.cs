using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Web
{
    public static class QueryStringHelper
    {
        public static string ToQueryString(string key, string value)
        {
            return $"?{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}";
        }

        public static string ToQueryString(this IDictionary<string, string> parameters)
        {
            var array = (from key in parameters.Keys
                         select $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(parameters[key])}")
                .ToArray();
            return $"?{string.Join("&", array)}";
        }

        public static Dictionary<string, string> ParseQueryString(this string queryString)
        {
            Dictionary<string, string> queryDict = new Dictionary<string, string>();
            var queryParts = queryString.TrimStart('?').Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in queryParts)
            {
                var parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    queryDict[WebUtility.UrlDecode(parts[0]).Trim()] = WebUtility.UrlDecode(parts[1]).Trim();
                }
                else if (parts.Length == 1)
                {
                    queryDict[parts[0].Trim()] = string.Empty;
                }
            }

            return queryDict;
        }

        public static string GetQueryParmeter(this string queryString, string key)
        {
            var navParams = queryString.ParseQueryString();
            if (navParams.ContainsKey(key))
            {
                return navParams[key];
            }

            return null;
        }

        /// <summary>
        /// A helper util to parse query params in an uri.
        /// </summary>
        /// <param name="queryString">format "?<name>=<value>&<name>=<value>" </param>
        /// <returns>dictionary of name-value pair of the query params</returns>
        public static Dictionary<string, string> GetQueryParams(string queryString, bool ignoreCase)
        {
            Dictionary<string, string> paramsDictionary = ignoreCase ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>();

            if (queryString == null
                || queryString.LastIndexOf('?') == -1)
            {
                return paramsDictionary;
            }

            string substring = queryString.Substring(queryString.LastIndexOf('?') + 1);

            string[] pairs = substring.Split('&');

            foreach (string piece in pairs)
            {
                string[] pair = piece.Split('=');
                if (pair != null
                    && pair.Length == 2)
                {
                    paramsDictionary.Add(pair[0], pair[1]);
                }
            }

            return paramsDictionary;
        }

        public static bool TryGetUlongFromQueryParams(Dictionary<string, string> queryParams, string paramName, out ulong value)
        {
            string resultAsString = null;
            ulong tempResult = 0;
            value = 0;

            if (queryParams != null &&
                !string.IsNullOrEmpty(paramName) &&
                queryParams.TryGetValue(paramName, out resultAsString) &&
                !string.IsNullOrWhiteSpace(resultAsString) &&
                ulong.TryParse(resultAsString, out tempResult))
            {
                value = tempResult;
                return true;
            }

            return false;
        }

        public static bool TryGetUintFromQueryParams(Dictionary<string, string> queryParams, string paramName, out uint value)
        {
            string resultAsString = null;
            uint tempResult = 0;
            value = 0;

            if (queryParams != null &&
                !string.IsNullOrEmpty(paramName) &&
                queryParams.TryGetValue(paramName, out resultAsString) &&
                !string.IsNullOrWhiteSpace(resultAsString) &&
                uint.TryParse(resultAsString, out tempResult))
            {
                value = tempResult;
                return true;
            }

            return false;
        }

        public static bool TryGetEnumFromQueryParams<TEnum>(Dictionary<string, string> queryParams, string paramName, out TEnum enumValue) where TEnum : struct
        {
            enumValue = default(TEnum);
            if (queryParams == null || paramName == null)
            {
                return false;
            }

            string stringValue;
            if (queryParams.TryGetValue(paramName, out stringValue))
            {
                return Enum.TryParse<TEnum>(stringValue, out enumValue);
            }

            return false;
        }
    }
}
