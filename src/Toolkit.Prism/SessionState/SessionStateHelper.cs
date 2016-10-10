using Prism.Windows.AppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Prism.SessionState
{
    public static class SessionStateHelper
    {
        public static T GetStateForKeyOrDefault<T>(this ISessionStateService sessionStateService, string key)
        {
            T value = default(T);
            object objValue = null;
            if (sessionStateService.SessionState.TryGetValue(key, out objValue) && objValue is T)
            {
                value = (T)objValue;
            }

            return value;
        }

        public static bool TryGetStateForKey<T>(this ISessionStateService sessionStateService, string key, out T value)
        {
            object objValue = null;
            if (sessionStateService.SessionState.TryGetValue(key, out objValue) && objValue is T)
            {
                value = (T)objValue;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }
    }
}
