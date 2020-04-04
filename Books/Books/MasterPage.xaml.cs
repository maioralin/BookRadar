using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : MasterDetailPage
    {
        public static NavigationPage NavPage;
        public MasterPage()
        {
            InitializeComponent();
            GlobalVars.Master = Detail;
            MPage.ListView.ItemSelected += ListView_ItemSelected;
            //Detail.Navigation.PushAsync(new MasterPageDetail());
            if (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.iOS) { Master.Icon = "swap.png"; MasterBehavior = MasterBehavior.Popover; }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            //Detail = new NavigationPage(page);
            Detail.Navigation.PushAsync(page);
            IsPresented = false;
            MasterBehavior = MasterBehavior.Popover;

            MPage.ListView.SelectedItem = null;
        }
    }
}