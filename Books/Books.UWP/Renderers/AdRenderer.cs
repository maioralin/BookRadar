using Books.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Microsoft.Advertising.WinRT.UI;
using Books.UWP.Renderers;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdRenderer))]
namespace Books.UWP.Renderers
{
    public class AdRenderer : ViewRenderer<AdMobView, AdControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                AdControl ad = new AdControl();
                ad.ApplicationId = "";
                ad.AdUnitId = "";
                if (Device.Idiom == TargetIdiom.Desktop)
                {
                    ad.Width = 728;
                    ad.Height = 90;
                }
                else
                {
                    ad.Width = 320;
                    ad.Height = 50;
                }
                ad.IsAutoRefreshEnabled = true;

                try
                {
                    SetNativeControl(ad);
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
