using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Toolkit.Common.Types
{
    public static class TypeConverterHelper
    {
        public static string GetHexStringFromUint(uint numberValue)
        {
            return "0x" + numberValue.ToString("X2", CultureInfo.InvariantCulture);
        }

        public static byte[] GetBytesFromString(string value)
        {
            return Enumerable.Range(0, value.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                         .ToArray();
        }

        public static string GetHexStringFromBytes(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", String.Empty);
        }

        /// <summary>
        /// Safely converts a string to an absolute Uri.
        /// </summary>
        /// <param name="uriString">string to be parsed into a Uri type.</param>
        /// <returns>Uri version of the string.</returns>
        public static Uri TryCreateUri(string uriString)
        {
            Uri uri = null;

            if (!string.IsNullOrWhiteSpace(uriString) && Uri.TryCreate(uriString, UriKind.Absolute, out uri))
            {
                return uri;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to parse a datetime value from the string.
        /// </summary>
        /// <param name="dateTimeString">string to parse</param>
        /// <param name="useXmlConversion">if true, uses XmlConvert. else, uses JSON.</param>
        /// <returns>value if found, otherwise null.</returns>
        public static DateTime? TryGetDateTimeValue(string dateTimeString, bool useXmlConversion = false, IFormatProvider formatter = null, DateTimeStyles parserStyles = DateTimeStyles.None)
        {
            DateTime? value = null;

            if (!string.IsNullOrEmpty(dateTimeString))
            {
                try
                {
                    if (useXmlConversion)
                    {
                        value = XmlConvert.ToDateTimeOffset(dateTimeString).DateTime;
                    }
                    else
                    {
                        DateTime parsed;
                        if (formatter != null && parserStyles != DateTimeStyles.None)
                        {
                            if (DateTime.TryParse(dateTimeString, formatter, parserStyles, out parsed))
                            {
                                value = parsed;
                            }
                        }
                        else
                        {
                            if (DateTime.TryParse(dateTimeString, out parsed))
                            {
                                value = parsed;
                            }
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Debug.Fail(ex.ToString());
                }
                catch (FormatException ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }

            return value;
        }

        /// <summary>
        /// Tries to parse a datetime value from the string.
        /// </summary>
        /// <param name="dateTimeString">string to parse</param>
        /// <param name="parserStyles">date time style to use</param>
        /// <returns>value if found, otherwise null.</returns>
        public static DateTime? TryGetDateTimeValue(string dateTimeString, DateTimeStyles parserStyles)
        {
            return TryGetDateTimeValue(dateTimeString, false, CultureInfo.InvariantCulture, parserStyles);
        }

        /// <summary>
        /// Tries to parse a timespan value from the string.
        /// </summary>
        /// <param name="dateTimeString">string to parse</param>
        /// <param name="useXmlConversion">if true, uses XmlConvert. else, uses JSON.</param>
        /// <returns>value if found, otherwise null.</returns>
        public static TimeSpan? TryGetTimeSpanValue(string timeSpanString, bool useXmlConversion = false)
        {
            TimeSpan? value = null;

            if (!string.IsNullOrEmpty(timeSpanString))
            {
                try
                {
                    if (useXmlConversion)
                    {
                        value = XmlConvert.ToTimeSpan(timeSpanString);
                    }
                    else
                    {
                        TimeSpan parsed;
                        if (TimeSpan.TryParse(timeSpanString, out parsed))
                        {
                            value = parsed;
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Debug.Fail(ex.ToString());
                }
                catch (FormatException ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }

            return value;
        }
    }
}
