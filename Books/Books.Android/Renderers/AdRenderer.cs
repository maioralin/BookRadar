using Android.Gms.Ads;
using Books.CustomControls;
using Books.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Widget;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Books.Droid
{
    // creates the renderer that 'maps' the cross-platform element to the native element
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdMobViewRenderer(Context context) : base(context)
        {
        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;

            if (dpHeight <= 400)
            {
                return 40;
            }
            if (dpHeight <= 720)
            {
                return 62;
            }
            return 102;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var adView = new AdView(Context)
                {
                    AdSize = AdSize.SmartBanner,
                    AdUnitId = ""
                };

                var requestbuilder = new AdRequest.Builder();

                adView.LoadAd(requestbuilder.Build());
                e.NewElement.HeightRequest = GetSmartBannerDpHeight();

                SetNativeControl(adView);
            }
        }

    }
}