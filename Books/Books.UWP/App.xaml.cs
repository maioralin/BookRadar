using Books.Responses;
using ImageCircle.Forms.Plugin.UWP;
using Microsoft.QueryStringDotNET;
using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Books.UWP
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
            this.InitializeComponent();
            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                Xamarin.Forms.Forms.Init(e, Rg.Plugins.Popup.Windows.Popup.GetExtraAssemblies());

                ImageCircleRenderer.Init();
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
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

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public static async void InitNotificationsAsync(string tag)
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var Tags = new HashSet<string>();
            Tags.Add(tag);

            var hub = new NotificationHub("BooksNotifications", "");
            var result = await hub.RegisterNativeAsync(channel.Uri, Tags);

        }

        public static async void UnregisterAsync()
        {
            var hub = new NotificationHub("BooksNotifications", "");
            await hub.UnregisterNativeAsync();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (args.Kind == ActivationKind.ToastNotification)
            {
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                if (toastArgs == null)
                    return;
                QueryString query = QueryString.Parse(toastArgs.Argument);
                if (query.Contains("action"))
                {
                    string action = query["action"];
                    switch(action)
                    {
                        case "requests":
                            GlobalVars.Notification = new NotificationInfo
                            {
                                NotificationAction = NotificationAction.Requests,
                                Param = string.Empty
                            };
                            break;
                        case "messages":
                            GlobalVars.Notification = new NotificationInfo
                            {
                                NotificationAction = NotificationAction.Messages,
                                Param = query["requestId"]
                            };
                            break;
                        default:
                            GlobalVars.Notification = new NotificationInfo
                            {
                                NotificationAction = NotificationAction.None,
                                Param = string.Empty
                            };
                            break;
                    }
                }
                if(GlobalVars.LoggedIn)
                {
                    if (GlobalVars.Notification != null)
                    {
                        if (GlobalVars.Notification.NotificationAction == NotificationAction.Requests)
                        {
                            GlobalVars.Notification = null;
                            var page = (Xamarin.Forms.Page)Activator.CreateInstance(typeof(MyRequestedBooks));
                            await GlobalVars.Master.Navigation.PushAsync(page);
                        }
                        else
                        {
                            if (GlobalVars.Notification.NotificationAction == NotificationAction.Messages)
                            {
                                int parameterId = int.Parse(GlobalVars.Notification.Param);
                                GlobalVars.Notification = null;
                                var resp = await RequestsHelper.MakeGetRequest<BookRequestResponse>($"borrow/getRequest/?RequestId={parameterId}");
                                if (resp != null)
                                {
                                    GlobalVars.CurrentRequest = resp.BookRequest;
                                    var page = (Xamarin.Forms.Page)Activator.CreateInstance(typeof(BookRequestPage));
                                    await GlobalVars.Master.Navigation.PushAsync(page);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (rootFrame == null)
                    {
                        // Create a Frame to act as the navigation context and navigate to the first page
                        rootFrame = new Frame();

                        rootFrame.NavigationFailed += OnNavigationFailed;

                        Xamarin.Forms.Forms.Init(toastArgs);
                        ImageCircleRenderer.Init();

                        if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                        {
                            //TODO: Load state from previously suspended application
                        }

                        // Place the frame in the current Window
                        Window.Current.Content = rootFrame;
                    }

                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage));
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
        }
    }
}
