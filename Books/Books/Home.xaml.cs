using Books.Requests;
using Books.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Books
{
    public partial class Home : ContentPage
    {
        public static bool _loginEnabled { get; set; }
        public Home()
        {
            InitializeComponent();
        }

        private async void TestClick(object sender, EventArgs e)
        {
            var resp1 = await RequestsHelper.MakeGetRequest<TestResponse>("values");
            var respDelete = await RequestsHelper.MakeDeleteRequest<TestResponse>("values/23");
            var testRequest = new TestRequest
            {
                Name = "Alin",
                Age = 23
            };
            var respPost = await RequestsHelper.MakePostRequest<TestResponse>("values", testRequest);
            var respPut = await RequestsHelper.MakePutRequest<TestResponse>("values/Maior", testRequest);
        }

        private async void FacebookLogin(object sender, EventArgs e)
        {
            if (_loginEnabled)
            {
                try
                {
                    _loginEnabled = false;
                    if (Device.RuntimePlatform == Device.UWP)
                    {
                        await DependencyService.Get<IPlatformSpecificFunctions>().FacebookLogin();
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new LoginPage());
                    }
                    if (GlobalVars.LoggedIn)
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
                                App.Current.MainPage = new MasterPage();
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    _loginEnabled = true;
                }
            }
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            _loginEnabled = false;
            try
            {
                await DependencyService.Get<IPlatformSpecificFunctions>().SilentLogin();
                if (GlobalVars.LoggedIn && (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.iOS))
                {
                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        while(!GlobalVars.LoadingDone)
                        {
                            await Task.Delay(1000);
                        }
                    }
                    var MasterPage = new MasterPage();
                    App.Current.MainPage = MasterPage;
                }
            }
            catch
            {

            }
            finally
            {
                _loginEnabled = true;
            }
        }
    }
}
