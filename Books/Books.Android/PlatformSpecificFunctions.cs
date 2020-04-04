using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Books.Droid;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Newtonsoft.Json;
using Books.OtherClasses;
using Books.Responses;
using Firebase.Iid;
using System.Net.Http;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificFunctions))]
namespace Books.Droid
{
    public class PlatformSpecificFunctions : Android.App.Activity, IPlatformSpecificFunctions
    {
        public PlatformSpecificFunctions() { }

        public Task<bool> FacebookLogin()
        {
            return null;
        }

        public async Task<bool> SilentLogin()
        {
            return await Helper.FacebookLogin();
        }

        public async Task<bool> Logout()
        {
            return await Helper.FacebookLogout();
        }

        public static class Helper
        {
            public async static Task<bool> FacebookLogin()
            {
                var account = AccountStore.Create(Forms.Context).FindAccountsForService("Facebook").FirstOrDefault();
                if (account != null)
                {
                    string accessToken = account.Properties["access_token"];
                    using (var client = new HttpClient())
                    {
                        Dictionary<string, string> postParameters = new Dictionary<string, string>();
                        postParameters.Add("grant_type", "facebook");
                        postParameters.Add("accesstoken", accessToken);

                        var content = new FormUrlEncodedContent(postParameters);

                        var response = client.PostAsync(RequestsHelper.authUrl, content).Result;

                        var responseString = response.Content.ReadAsStringAsync().Result;

                        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);

                        RequestsHelper.Bearer = authResponse.access_token;
                    }
                    var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=id,last_name,gender,birthday,picture,email"), null, account);
                    await request.GetResponseAsync().ContinueWith(async t =>
                    {
                        if (t.IsFaulted)
                        {
                            Home._loginEnabled = true;
                        }
                        else
                        {
                            GlobalVars.LoggedIn = true;
                            string json = t.Result.GetResponseText();
                            UserInfo extendedUser = JsonConvert.DeserializeObject<UserInfo>(json);
                            GlobalVars.FacebookDetails = new FacebookDetails
                            {
                                Email = extendedUser.Email,
                                ID = extendedUser.Id,
                                Gender = extendedUser.Gender,
                                LastName = extendedUser.Last_Name,
                                ProfilePicture = extendedUser.Picture.Data.Url
                            };

                            var resp2 = await RequestsHelper.MakeGetRequest<UserDetailsResponse>($"facebook/GetUserDetailsByFacebookId/?facebookId={GlobalVars.FacebookDetails.ID}");
                            if (resp2.ErrorCode == 0)
                            {
                                GlobalVars.MyReferralCode = resp2.Info.MyReferralCode;
                                GlobalVars.InviteCode = resp2.Info.InviteCode;
                                GlobalVars.UserId = resp2.Info.UserId;
                                GlobalVars.PurchaseId = resp2.Info.PurchaseId;
                                List<string> tags = new List<string>();
                                tags.Add(GlobalVars.UserId.ToString());
                                await RequestsHelper.NotificationHubRegister("gcm", FirebaseInstanceId.Instance.Token, tags);
                                var MasterPage = new MasterPage();
                                App.Current.MainPage = MasterPage;
                            }
                        }
                    });
                }
                else
                {
                    Home._loginEnabled = true;
                }
                return true;
            }

            public async static Task<bool> FacebookLogout()
            {
                var account = AccountStore.Create(Forms.Context).FindAccountsForService("Facebook").FirstOrDefault();
                if (account != null)
                {
                    AccountStore.Create(Forms.Context).Delete(account, "Facebook");
                    await RequestsHelper.NotificationHubDelete();
                    GlobalVars.LoggedIn = false;
                }

                Home._loginEnabled = true;

                return true;
            }
        }
    }
}