using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Toolkit.Xaml.Dispatcher
{
    public static class DispatcherHelper
    {
        private static object _lock = new object();
        private static CoreDispatcher _dispatcher;

        private static CoreDispatcher Dispatcher
        {
            get
            {
                CoreDispatcher dispatcher = null;

                lock (_lock)
                {
                    // Get the existing dispatcher under the lock
                    dispatcher = _dispatcher;
                }

                if (dispatcher == null)
                {
                    // No dispatcher; attempt to access the dispatcher (outside of the lock)
                    dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                    lock (_lock)
                    {
                        if (_dispatcher == null)
                        {
                            // Store the dispatcher for future use
                            _dispatcher = dispatcher;
                        }
                        else
                        {
                            // Dispatcher was set before we were able to retrieve; throw away the new one and use the
                            // stored one
                            dispatcher = _dispatcher;
                        }
                    }
                }

                return dispatcher;
            }
        }

        public static Task RunOnUIThreadAsync(DispatchedHandler handler)
        {
            return RunOnUIThreadAsync(CoreDispatcherPriority.Normal, handler);
        }

        public static Task RunOnUIThreadAsync(CoreDispatcherPriority priority, DispatchedHandler handler)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                var dispatcher = Dispatcher;

                if (dispatcher.HasThreadAccess)
                {
                    handler.Invoke();
                }
                else
                {
                    var task = dispatcher.RunAsync(priority, () => { handler(); }).AsTask();

                    return task;
                }
            }

            return Task.FromResult<object>(null);
        }
    }
}
