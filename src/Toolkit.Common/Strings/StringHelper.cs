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
        /// <summary>
        /// Returns a result string in which arguments are formatted by using the conventions
        /// of the invariant culture.
        ///
        /// Note: This is intentionally reimplemented here to allow a single using static for current
        /// and invariant cultures.
        ///
        /// Ex.:
        ///     using static Toolkit.Common.Strings.StringHelper;
        ///     InvariantCulture($"{UnlockedCount}/{TotalCount}");
        /// </summary>
        /// <param name="formattable">The object to convert to a result string.</param>
        /// <returns>The string that results from formatting the current instance by using the conventions of the invariant culture.</returns>
        public static string InvariantCulture(FormattableString formattable)
        {
            return FormattableString.Invariant(formattable);
        }

        /// <summary>
        /// Returns a result string in which arguments are formatted by using the conventions
        /// of the current culture.
        ///
        /// Ex.:
        ///     using static Toolkit.Common.Strings.StringHelper;
        ///     CurrentCulture($"{UnlockedCount}/{TotalCount}");
        /// </summary>
        /// <param name="formattable">The object to convert to a result string.</param>
        /// <returns>The string that results from formatting the current instance by using the conventions of the current culture.</returns>
        public static string CurrentCulture(FormattableString formattable)
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

        public static IEnumerable<int> AllIndexesOf(this string str, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var indexes = new List<int>();

            for (int i = 0; i < str.Length;)
            {
                var index = str.IndexOf(value, i, comparison);
                if (index == -1)
                {
                    break;
                }

                indexes.Add(index);
                i += index + 1;
            }

            return indexes;
        }
    }
}
