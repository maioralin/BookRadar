using Books;
using Books.Responses;
using Foundation;
using Google.MobileAds;
using ImageCircle.Forms.Plugin.iOS;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System;
using System.Collections.Generic;
using UIKit;

namespace Books.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            KeyboardOverlapRenderer.Init();
            Corcav.Behaviors.Infrastructure.Init();
            MobileAds.Configure("");
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            LoadApplication(new Books.App());

            return base.FinishedLaunching(app, options);
        }

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //await RequestsHelper.NotificationHubDelete();
            List<string> tags = new List<string>();
            tags.Add(GlobalVars.UserId.ToString());
            var deviceTokenString = deviceToken.ToString().Replace("<", "").Replace(">", "").Replace(" ", "");
            await RequestsHelper.NotificationHubRegister("apns", deviceTokenString, tags);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            var alert = new UIAlertView("Failed to register for notifications", "Notification registration failed! Try again!", null, "OK", null);

            alert.Show();
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }

        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (UIApplication.SharedApplication.ApplicationState.Equals(UIApplicationState.Active))
            {
            }
            else
            {
                string action = string.Empty;
                if(options != null && options.ContainsKey(new NSString("action")))
                {
                    action = (options.ObjectForKey(new NSString("action")) as NSString).ToString();
                }
                switch (action)
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
                            Param = (options.ObjectForKey(new NSString("requestId")) as NSString).ToString()
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
        }
    }
}
