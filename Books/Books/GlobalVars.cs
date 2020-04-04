using Books.OtherClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Books
{
    public static class GlobalVars
    {
        public static bool LoggedIn { get; set; }
        public static FacebookDetails FacebookDetails { get; set; }
        public static Guid UserId { get; set; }
        public static List<Book> Books { get; set; }
        public static Page Master { get; set; }
        public static LocationDetails GeoLocation { get; set; }
        public static Book VisitedBook { get; set; }
        public static BookRequest CurrentRequest { get; set; }
        public static DeviceRegistration deviceRegistration { get; set; }
        public static NotificationInfo Notification { get; set; }
        public static string MyReferralCode { get; set; }
        public static string InviteCode { get; set; }
        public static ChatClient ChatClient { get; set; }
        public static string PurchaseId { get; set; }
        public static bool LoadingDone { get; set; }
    }

    public class FacebookDetails
    {
        public string Email { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string LastName { get; set; }
    }

    public class LocationDetails
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class NotificationInfo
    {
        public NotificationAction NotificationAction { get; set; }
        public string Param { get; set; }
    }

    public enum NotificationAction
    {
        None = 0,
        Requests = 1,
        Messages = 2
    }
}
