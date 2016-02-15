using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Toolkit.Web
{
    public static class UriExtensions
    {
        /// <summary>
        /// Parses the URI's query string and returns the resulting parsed object.
        /// </summary>
        /// <param name="uri">The URI whose query string to parse.</param>
        /// <returns>The resulting parsed object.</returns>
        public static WwwFormUrlDecoder QueryParsed(this Uri uri)
        {
            var queryString = uri.Query;
            if (string.IsNullOrEmpty(queryString))
            {
                queryString = "?";
            }

            return new WwwFormUrlDecoder(queryString);
        }

        /// <summary>
        /// Attempts to retrieve the value of the first query parameter of the specified name.
        /// </summary>
        /// <param name="uri">The URI whose query parameter value to retrieve.</param>
        /// <param name="queryParamName">The parameter to retrieve.</param>
        /// <returns>The value of the parameter, or null if no such value exists.</returns>
        public static string TryGetFirstQueryParamValueByName(this Uri uri, string queryParamName)
        {
            var queryParams = QueryParsed(uri);

            foreach (var param in queryParams)
            {
                if (string.Equals(param.Name, queryParamName, StringComparison.OrdinalIgnoreCase))
                {
                    return param.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads all name/value pairs from the specified parsed query into a dictionary.
        /// </summary>
        /// <param name="parsedQuery">The parsed query to transform.</param>
        /// <returns>A name/value pairing for query strings.  If there are duplicate query values, only the first
        /// value for each such value is returned.</returns>
        public static IDictionary<string, string> AsDictionary(this WwwFormUrlDecoder parsedQuery)
        {
            var result = new Dictionary<string, string>();
            foreach (var pair in parsedQuery)
            {
                if (!result.ContainsKey(pair.Name))
                {
                    result.Add(pair.Name, pair.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds a query value to an existing URI string.
        /// </summary>
        public static string AddQueryParam(string existingUrl, string paramName, string paramValue)
        {
            // If there isn't a ?, then use that first, otherwise use & for subsequent params
            var leadingChar = existingUrl.Contains("?") ? "&" : "?";
            return string.Format($"{existingUrl}{leadingChar}{paramName}={paramValue}");
        }
    }
}
