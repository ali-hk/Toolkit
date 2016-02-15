using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace Toolkit.Uwp
{
    public static class UIThreadHelper
    {
        public static void AssertIsUIThread()
        {
            Debug.Assert(CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess, "This method must be called on the UI thread.");
        }
    }
}
