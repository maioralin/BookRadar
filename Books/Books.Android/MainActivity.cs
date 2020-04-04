using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.CurrentActivity;
using Android.Content;
using Plugin.InAppBilling;

namespace Books.Droid
{
    [Activity(Label = "BookRadar", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            if(Intent != null)
            {
                string action = Intent.GetStringExtra("action");
                if(!string.IsNullOrEmpty(action))
                {
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
                                Param = Intent.GetStringExtra("requestId")
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
            IsPlayServicesAvailable();

            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, "");
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ImageCircleRenderer.Init();
            LoadApplication(new App());
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                string error = string.Empty;
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    error = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    error = "This device is not supported";
                }
                Toast.MakeText(ApplicationContext, error, ToastLength.Long).Show();
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}

