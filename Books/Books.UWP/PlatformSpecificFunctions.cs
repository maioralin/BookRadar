using Books.UWP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using winsdkfb;
using Books;
using winsdkfb.Graph;
using Books.OtherClasses;
using Books.Responses;
using Windows.Networking.PushNotifications;
using System.Net.Http;
using Books.Requests;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificFunctions))]
namespace Books.UWP
{
    public class PlatformSpecificFunctions : IPlatformSpecificFunctions
    {
        public async Task<bool> FacebookLogin()
        {
            return await Helper.FacebookLogin();
        }

        public async Task<bool> SilentLogin()
        {
            return await Helper.FacebookLogin(true);
        }

        public async Task<bool> Logout()
        {
            return await Helper.FacebookLogout();
        }

        public static class Helper
        {
            public async static Task<bool> FacebookLogin(bool silent = false)
            {
                FBSession sess = FBSession.ActiveSession;
                sess.FBAppId = "";
                sess.WinAppId = "";
                string SID = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
                sess.SetWebViewRedirectUrl("https://bookradar.net/privacy/loginsuccess", string.Empty);

                // Add permissions required by the app
                List<String> permissionList = new List<String>();
                permissionList.Add("public_profile");
                permissionList.Add("email");

                //await sess.LogoutAsync();

                FBPermissions permissions = new FBPermissions(permissionList);

                FBResult result = await sess.LoginAsync(permissions, silent ? SessionLoginBehavior.Silent : SessionLoginBehavior.WebAuth);
                if (result.ErrorInfo != null && !silent)
                {
                    await sess.LogoutAsync();
                    await FacebookLogin(true);
                    return true;
                }

                GlobalVars.LoggedIn = FBSession.ActiveSession.LoggedIn;

                PropertySet parameters = new PropertySet();
                parameters.Add("fields", "id,last_name,gender,birthday,picture,email");

                // Set Graph api path to get data about this user
                string path = "/me/";

                // Create the request to send to the Graph API, also specify the format we're expecting (In this case a json that can be parsed into FBUser)
                FBSingleValue sval = new FBSingleValue(path, parameters,
                  new FBJsonClassFactory(JsonConvert.DeserializeObject<UserInfo>));

                // Do the actual request
                FBResult fbresult = await sval.GetAsync();

                if(fbresult.Succeeded)
                {

                    var extendedUser = ((UserInfo)fbresult.Object);
                    using (var client = new HttpClient())
                    {
                        Dictionary<string, string> postParameters = new Dictionary<string, string>();
                        postParameters.Add("grant_type", "facebook");
                        postParameters.Add("accesstoken", sess.AccessTokenData.AccessToken);

                        var content = new FormUrlEncodedContent(postParameters);

                        var response = await client.PostAsync(RequestsHelper.authUrl, content);

                        var responseString = await response.Content.ReadAsStringAsync();

                        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);

                        RequestsHelper.Bearer = authResponse.access_token;
                    }

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
                }
                else
                {
                    Home._loginEnabled = true;
                }

                return true;
            }

            public async static Task<bool> FacebookLogout()
            {
                FBSession sess = FBSession.ActiveSession;
                await sess.LogoutAsync();

                await RequestsHelper.NotificationHubDelete();

                GlobalVars.LoggedIn = false;

                Home._loginEnabled = true;

                return true;
            }
        }
    }
}
