using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using Android.Graphics;
using System.Collections.Generic;

namespace Books.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseNotificationService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            Log.Debug(TAG, "From: " + message.From);
            string message2 = message.Data["message"];
            string title = message.Data["title"];
            Dictionary<string, string> param = new Dictionary<string, string>();
            foreach(var p in message.Data)
            {
                if(p.Key != "message" && p.Key != "title")
                {
                    param.Add(p.Key, p.Value);
                }
            }
            SendNotification(message2, title, param);
        }

        void SendNotification(string messageBody, string title, Dictionary<string, string> param)
        {
            var intent = new Intent(this, typeof(MainActivity));
                
            foreach(var p in param)
            {
                intent.PutExtra(p.Key, p.Value);
            }
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);


            var notificationManager = NotificationManager.FromContext(this);
            string channelId = "channel_01";
            string name = "books_channel";
            String description = "BookRadarChanner";
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                NotificationImportance importance = NotificationManager.ImportanceDefault;
                NotificationChannel notificationChannel = new NotificationChannel(channelId, name, importance);
                notificationChannel.EnableLights(true);
                notificationChannel.LightColor = Color.Red;
                notificationChannel.EnableVibration(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            var notificationBuilder = new Notification.Builder(this, channelId)
                .SetSmallIcon(Resource.Drawable.notif)
                .SetContentTitle(title)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}