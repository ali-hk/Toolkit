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
        public static WwwFormUrlDecoder ParseQueryString(this Uri uri)
        {
            var queryString = uri.Query;
            if (string.IsNullOrEmpty(queryString))
            {
                queryString = "?";
            }

            return new WwwFormUrlDecoder(queryString);
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
