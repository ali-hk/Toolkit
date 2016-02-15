using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Web
{
    public static class QueryStringHelper
    {
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
