using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Books.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListView))]
namespace Books.iOS.Renderers
{
    public class CustomListView: ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            this.Control.TableFooterView = new UIView();
        }
    }
}