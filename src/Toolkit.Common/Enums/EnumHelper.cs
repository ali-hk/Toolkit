using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Enums
{
    public static class EnumHelper
    {
        public static T EnumFromString<T>(string stringValue, T defaultValue = default(T)) where T : struct
        {
            T returnValue = defaultValue;
            if (defaultValue is Enum && Enum.TryParse<T>(stringValue, true, out returnValue))
            {
                return returnValue;
            }

            return defaultValue;
        }
    }
}
