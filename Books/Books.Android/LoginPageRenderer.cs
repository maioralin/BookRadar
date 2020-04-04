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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Books;
using Books.Droid;
using Xamarin.Auth;
using Newtonsoft.Json;
using Books.OtherClasses;
using Firebase.Iid;
using System.Net.Http;
using Books.Responses;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace Books.Droid
{
    public class LoginPageRenderer : PageRenderer
    {
        public LoginPageRenderer(Context context): base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            var activity = Context as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "", // your OAuth2 client id
                scope: "public_profile+email", // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://www.facebook.com/v2.10/dialog/oauth"), // the auth URL for the service
                redirectUrl: new Uri("https://bookradar.net/privacy/loginsuccess")); // the redirect URL for the service

            auth.Completed += (sender, eventArgs) => {
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
                    request.GetResponseAsync().ContinueWith(t => {
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

                            GlobalVars.LoggedIn = true;

                            //MainActivity.RegisterWithGCM();

                            AccountStore.Create(this.Context).Save(eventArgs.Account, "Facebook");
                            GlobalVars.deviceRegistration = new DeviceRegistration
                            {
                                Platform = "gcm",
                                Handle = FirebaseInstanceId.Instance.Token,
                                Tags = new List<string>()
                            };
                            LoginPage.LoginSuccess();
                        }
                    });
                }
                else
                {
                    Home._loginEnabled = true;
                    // The user cancelled
                }
            };

            activity.StartActivity(auth.GetUI(activity));
        }
    }
}