using Books.OtherClasses;
using Books.Requests;
using Books.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Xamarin.Forms;

namespace Books
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LoginPage.LoginSucceeded += HandleLoginSucceeded;
            LoginPage.LoginCancelled += CancelLoginAction;
            SetTheme();

            MainPage = new Books.Home();
        }

        private void SetTheme()
        {
            App.Current.Resources["backgroundColor"] = Color.FromHex("1E1E1E");
            App.Current.Resources["menuColor"] = Color.FromHex("272727");
            App.Current.Resources["labelColor"] = Color.FromHex("DDD");
            App.Current.Resources["entryColor"] = Color.FromHex("484848");
            App.Current.Resources["buttonColor"] = Color.FromHex("585858");
            App.Current.Resources["buttonText"] = Color.FromHex("DDD");
            App.Current.Resources["entryText"] = Color.FromHex("DDD");
            App.Current.Resources["buttonBorderColor"] = Color.FromHex("888");
            App.Current.Resources["buttonBorderRadius"] = 10;
        }

        public async void HandleLoginSucceeded(object sender, EventArgs e)
        {
            var facebookLoginRequest = new FacebookLoginRequest
            {
                Email = GlobalVars.FacebookDetails.Email,
                ID = GlobalVars.FacebookDetails.ID,
                LastName = GlobalVars.FacebookDetails.LastName,
                Picture = GlobalVars.FacebookDetails.ProfilePicture,
                Gender = GlobalVars.FacebookDetails.Gender
            };
            var resp = await RequestsHelper.MakePostRequest<FacebookLoginResponse>("facebook/login", facebookLoginRequest);
            if (resp.ErrorCode == 0)
            {
                var resp2 = await RequestsHelper.MakeGetRequest<UserDetailsResponse>($"facebook/GetUserDetailsByFacebookId/?facebookId={GlobalVars.FacebookDetails.ID}");
                if (resp2.ErrorCode == 0)
                {
                    GlobalVars.MyReferralCode = resp2.Info.MyReferralCode;
                    GlobalVars.InviteCode = resp2.Info.InviteCode;
                    GlobalVars.UserId = resp2.Info.UserId;
                    GlobalVars.PurchaseId = resp2.Info.PurchaseId;
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        List<string> tags = new List<string>();
                        tags.Add(GlobalVars.UserId.ToString());
                        await RequestsHelper.NotificationHubRegister(GlobalVars.deviceRegistration.Platform, GlobalVars.deviceRegistration.Handle, tags);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                        Current.MainPage = new MasterPage());
                }
            }
        }

        public void CancelLoginAction(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            MainPage = new Home());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
