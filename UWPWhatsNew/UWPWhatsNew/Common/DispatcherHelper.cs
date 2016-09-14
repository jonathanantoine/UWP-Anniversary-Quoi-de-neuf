using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
#if WINDOWS_PHONE

#else

#endif

namespace UWPWhatsNew.Common
{
    public partial class DispatcherHelper
    {

#if WINDOWS_PHONE
        public static System.Windows.Threading.Dispatcher UIDispatcher { get; private set; }

#else
        public static CoreDispatcher UIDispatcher { get; private set; }

#endif

        /// <summary>
        /// Initialize the dispatcher. Must be called during application' startup.
        /// </summary>
        public static void Initialize()
        {
            if (UIDispatcher != null)
            {
                return;
            }
#if WINDOWS_PHONE
            UIDispatcher = System.Windows.Deployment.Current.Dispatcher;

#else
            UIDispatcher = Window.Current.Dispatcher;

#endif
        }

        /// <summary>
        /// Invoke the following action on the appropriate thread.
        /// </summary>
        public static async Task CheckBeginInvokeOnUIAsync(Action action)
        {
            if (UIDispatcher == null)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                action();
            }

            if (UIDispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());

            }

        }
    }

}