using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Books;
using Books.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Auth;
using Books.OtherClasses;
using Newtonsoft.Json;
using System.Net.Http;
using Books.Responses;
using UserNotifications;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace Books.iOS
{
    public class LoginPageRenderer : PageRenderer
    {
        public LoginPageRenderer() : base()
        {

        }
        private static bool login = false;
        public override void ViewDidAppear(bool animated)
        {
            if (!login)
            {
                login = true;
                base.ViewDidAppear(animated);

                var auth = new OAuth2Authenticator(
                    clientId: "", // your OAuth2 client id
                    scope: "public_profile+email", // the scopes for the particular API you're accessing, delimited by "+" symbols
                    authorizeUrl: new Uri("https://www.facebook.com/v2.10/dialog/oauth"), // the auth URL for the service
                    redirectUrl: new Uri("https://bookradar.net/privacy/loginsuccess")); // the redirect URL for the service

                auth.Completed += (sender, eventArgs) =>
                {
                    if (eventArgs.IsAuthenticated)
                    {
                        string accessToken = eventArgs.Account.Properties["access_token"];
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
                        var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=id,last_name,gender,birthday,picture,email"), null, eventArgs.Account);
                        request.GetResponseAsync().ContinueWith(async t =>
                        {
                            if (t.IsFaulted)
                            {
                                Home._loginEnabled = true;
                            }
                            else
                            {
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
                                }

                                GlobalVars.LoggedIn = true;

                                AccountStore.Create().Save(eventArgs.Account, "Facebook");
                                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                                                                (granted, error) =>
                                                                {
                                                                    if (granted)
                                                                        InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                                                                });
                                LoginPage.LoginSuccess();
                                DismissViewController(true, null);
                            }
                        });
                    }
                    else
                    {
                        Home._loginEnabled = true;
                        // The user cancelled
                    }
                };

                PresentViewController((UIViewController)auth.GetUI(), true, null);
            }

        }
    }
}