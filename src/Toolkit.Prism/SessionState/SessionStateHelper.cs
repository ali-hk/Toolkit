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
        public static T GetStateForKey<T>(this ISessionStateService sessionStateService, string key)
        {
            T value = default(T);
            if (sessionStateService.SessionState.ContainsKey(key))
            {
                try
                {
                    value = (T)sessionStateService.SessionState[key];
                }
                catch (InvalidCastException)
                {
                    value = default(T);
                }
            }

            return value;
        }
    }
}
