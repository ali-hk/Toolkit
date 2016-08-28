using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Xaml.Navigation
{
    public static class NavigationParameterHelper
    {
        public static string ToNavigationString(string key, string value)
        {
            return $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}";
        }

        public static string ToNavigationString(this Dictionary<string, string> parameters)
        {
            var array = (from key in parameters.Keys
                         select $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(parameters[key])}")
                .ToArray();
            return string.Join("&", array);
        }

        public static Dictionary<string, string> ParseNavigationString(this string navParamString)
        {
            Dictionary<string, string> queryDict = new Dictionary<string, string>();
            foreach (string token in navParamString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    queryDict[WebUtility.UrlDecode(parts[0]).Trim()] = WebUtility.UrlDecode(parts[1]).Trim();
                }
                else
                {
                    queryDict[parts[0].Trim()] = string.Empty;
                }
            }

            return queryDict;
        }

        public static string ParseNavigationString(this string navParamString, string key)
        {
            var navParams = navParamString.ParseNavigationString();
            if (navParams.ContainsKey(key))
            {
                return navParams[key];
            }

            return null;
        }

        public static string ParseNavigationString(this object navParamString, string key)
        {
            if (navParamString is string)
            {
                return ParseNavigationString(navParamString as string, key);
            }

            return null;
        }
    }
}