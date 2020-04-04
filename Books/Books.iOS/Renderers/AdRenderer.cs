using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Books.CustomControls;
using Books.iOS;
using Foundation;
using Google.MobileAds;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobRenderer))]
namespace Books.iOS
{
    public class AdMobRenderer : ViewRenderer
    {
        BannerView adView;
        bool viewOnScreen;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (e.OldElement == null)
            {
                adView = new BannerView(AdSizeCons.SmartBannerPortrait)
                {
                    AdUnitID = "",
                    RootViewController = GetRootViewController()
                };

                adView.AdReceived += (sender, args) =>
                {
                    if (!viewOnScreen) this.AddSubview(adView);
                    viewOnScreen = true;
                };

                var request = Request.GetDefaultRequest();

                e.NewElement.HeightRequest = GetSmartBannerDpHeight();
                adView.LoadRequest(request);

                base.SetNativeControl(adView);
            }
        }

        private UIViewController GetRootViewController()
        {
            foreach (UIWindow window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController != null)
                {
                    return window.RootViewController;
                }
            }

            return null;
        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = (double)UIScreen.MainScreen.Bounds.Height;

            if (dpHeight <= 400) return 32;
            if (dpHeight <= 720) return 50;
            return 90;
        }
    }

}