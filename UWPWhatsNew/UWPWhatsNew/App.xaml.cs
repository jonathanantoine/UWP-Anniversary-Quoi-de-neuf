﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UWPWhatsNew.Common;
using UWPWhatsNew.Views.Partials;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Application = Windows.UI.Xaml.Application;

namespace UWPWhatsNew
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            MessageService.ClearAllPreviousToast();
            EnteredBackground += App_OnEnteredBackground;
            LeavingBackground += App_OnLeavingBackground;
            Suspending += OnSuspending;
            Resuming += OnResuming;
        }
#if LIFECYCLE_EVENTS
        private static int _lifecycleEventCount = 0;
#endif
        private void App_OnLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            var def = e.GetDeferral();

#if LIFECYCLE_EVENTS
            MessageService.DisplayToastMessage(_lifecycleEventCount++ + " LEAVING BACKGROUND");
#endif
            def.Complete();
        }

        private void App_OnEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var def = e.GetDeferral();
#if LIFECYCLE_EVENTS

            var usage = MemoryManager.AppMemoryUsage / (1024 * 1024) + " Mo";
            var limit = MemoryManager.AppMemoryUsageLimit / (1024 * 1024) + " Mo";
            var percent = (int)(MemoryManager.AppMemoryUsage / (double)MemoryManager.AppMemoryUsageLimit * 100) + "%";

            MessageService.DisplayToastMessage(_lifecycleEventCount++ + " ENTERED BACKGROUND :"
                + Environment.NewLine + usage + " of " + limit + " - " + percent);
#endif

            def.Complete();
        }


        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
#if LIFECYCLE_EVENTS
            MessageService.DisplayToastMessage(_lifecycleEventCount++ + " SUSPENDING");
#endif

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
#if LIFECYCLE_EVENTS
            MessageService.DisplayToastMessage(_lifecycleEventCount++ + " RESUMING");
#endif
        }

        public Frame RootFrame { get; set; }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (RootFrame == null)
            {
                return;
            }
            base.OnBackgroundActivated(args);
            var instance = args.TaskInstance;
            
            //Démarrage du traitement AppService
            var details = instance.TriggerDetails as AppServiceTriggerDetails;
            if (details != null)
            {
                CustomSnowFallFromAppServiceAsync(instance, details);
            }
        }

        private void CustomSnowFallFromAppServiceAsync(
            IBackgroundTaskInstance instance,
            AppServiceTriggerDetails details)
        {
            var def = instance.GetDeferral();
            instance.Canceled += (_, __) => { def.Complete(); };
            details.AppServiceConnection.RequestReceived += (_, message) =>
            {
                var messageDef = message.GetDeferral();
                try
                {
                    try
                    {
                        // lecture du message
                        var targetUri = message.Request.Message.FirstOrDefault(kVp => kVp.Key == "targetUri").Value?.ToString();
                        if (!string.IsNullOrEmpty(targetUri))
                        {

                            // Uri locale = pas de téléchargement
                            if (targetUri.StartsWith("ms-appx"))
                            {
                                DispatcherHelper.CheckBeginInvokeOnUIAsync(() => SnowFallAsync(true, new BitmapImage(new Uri(targetUri))));
                                return;
                            }

                            DispatcherHelper.CheckBeginInvokeOnUIAsync(
                                async () =>
                                {
                                    // téléchargement local de l'image
                                    var image = await Microsoft.Toolkit.Uwp.UI.ImageCache.GetFromCacheAsync(new Uri(targetUri));

                                    SnowFallAsync(true, image);
                                });

                        }

                    }
                    finally
                    {
                        // fermeture du message
                        messageDef.Complete();
                    }
                }
                finally
                {
                    // On ne fera pas d'autres communications
                    def.Complete();
                }
            };

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PrelaunchActivated)
            {
                return;
            }

            InitializeFrame(e);
        }

        private void InitializeFrame(IActivatedEventArgs e)
        {
            if (RootFrame != null)
            {
                return;
            }
            DispatcherHelper.Initialize();
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = ((SolidColorBrush)Resources["TitleBarButtonColorBrush"]).Color;
            titleBar.BackgroundColor = ((SolidColorBrush)Resources["MainColorBrush"]).Color;
            titleBar.ForegroundColor = titleBar.ButtonForegroundColor;

            RootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                RootFrame.Navigate(typeof(Shell), (e as LaunchActivatedEventArgs)?.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();

        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            bool shouldDelay = RootFrame == null;
            InitializeFrame(args);

            var protocol = args as ProtocolActivatedEventArgs;
            if (protocol?.Uri.Host?.ToLowerInvariant() == "snowfall")
            {
                SnowFallAsync(shouldDelay);
            }
            //td2016whatsnew://snowfall
        }

        private static readonly Random _Random = new Random((int)DateTime.UtcNow.Ticks);
        private async Task SnowFallAsync(bool shouldDelay, BitmapImage image = null)
        {
            if (shouldDelay)
            {
                await DispatcherHelper.UIDispatcher.RunIdleAsync(_ => { });
            }

            var panel = (RootFrame.ContentTemplateRoot as Page)?.Content as Grid;
            for (int i = 0; i < 12; i++)
            {
                await Task.Delay(_Random.Next(50, 200));
                panel?.Children.Add(new SnowFallUserControl(image));
                panel?.Children.Add(new SnowFallUserControl(image));
            }
        }
    }
}
