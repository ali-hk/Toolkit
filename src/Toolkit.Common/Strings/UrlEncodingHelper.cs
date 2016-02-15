using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Strings
{
    public static class UrlEncodingHelper
    {
        /// <summary>
        /// Uses Uri.EscapeDataString() based on recommendations on MSDN
        /// http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        internal static string UrlEncode(this string input)
        {
            const int MAXLENGTH = 32766;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length <= MAXLENGTH)
            {
                return Uri.EscapeUriString(input);
            }

            StringBuilder sb = new StringBuilder(input.Length * 2);
            int index = 0;
            while (index < input.Length)
            {
                int length = Math.Min(input.Length - index, MAXLENGTH);
                string subString = input.Substring(index, length);
                sb.Append(Uri.EscapeUriString(subString));
                index += subString.Length;
            }

            return sb.ToString();
        }
    }
}
