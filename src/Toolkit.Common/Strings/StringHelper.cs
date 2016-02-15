using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Strings
{
    public static class StringHelper
    {
        public static string InvariantCulture(this FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(CultureInfo.InvariantCulture);
        }

        public static string CurrentCulture(this FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(CultureInfo.CurrentCulture);
        }

        public static bool NotNullAndEquals(string leftString, string rightString, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return !string.IsNullOrWhiteSpace(leftString) && string.Equals(leftString, rightString, comparisonType);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
