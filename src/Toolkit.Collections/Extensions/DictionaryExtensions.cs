using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Collections.Extensions
{
    /// <summary>
    /// Extension methods for retrieving keyed values from an <see cref="IDictionary{string, object}" />,
    /// <see cref="Windows.Foundation.Collections.PropertySet"/> or <see cref="Windows.Foundation.Collections.ValueSet"/>
    /// </summary>
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value = dictionary[key];
            return (T)value;
        }

        public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key)
        {
            try
            {
                object value = null;
                if (dictionary.TryGetValue(key, out value))
                {
                    return (T)value;
                }
                else
                {
                    return default(T);
                }
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            bool returnValue = false;
            try
            {
                object valueObj = null;
                if (dictionary.TryGetValue(key, out valueObj))
                {
                    value = (T)valueObj;
                    returnValue = true;
                }
                else
                {
                    value = default(T);
                }
            }
            catch (InvalidCastException)
            {
                value = default(T);
            }

            return returnValue;
        }

        public static T ParseValue<T>(this IDictionary<string, object> dictionary, string key, IFormatProvider formatProvider = null)
        {
            object value = dictionary[key];
            return (T)Convert.ChangeType(value, typeof(T), formatProvider);
        }

        public static T ParseValueOrDefault<T>(this IDictionary<string, object> dictionary, string key, IFormatProvider formatProvider = null)
        {
            try
            {
                object value = null;
                if (dictionary.TryGetValue(key, out value))
                {
                    var parsedValue = (T)Convert.ChangeType(value, typeof(T), formatProvider);
                    return parsedValue;
                }
                else
                {
                    return default(T);
                }
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        public static bool TryParseValue<T>(this IDictionary<string, object> dictionary, string key, out T value, IFormatProvider formatProvider = null)
        {
            bool returnValue = false;
            try
            {
                object valueObj = null;
                if (dictionary.TryGetValue(key, out valueObj))
                {
                    value = (T)Convert.ChangeType(valueObj, typeof(T), formatProvider);
                    returnValue = true;
                }
                else
                {
                    value = default(T);
                }
            }
            catch (Exception)
            {
                value = default(T);
            }

            return returnValue;
        }
    }
}
