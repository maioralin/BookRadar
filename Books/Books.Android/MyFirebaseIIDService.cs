using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Books.OtherClasses;
using Firebase.Iid;
using Newtonsoft.Json;

namespace Books.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public async override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            List<string> tags = new List<string>();
            tags.Add(GlobalVars.UserId.ToString());
            await RequestsHelper.NotificationHubRegister("gcm", refreshedToken, tags);
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
        }
    }
}