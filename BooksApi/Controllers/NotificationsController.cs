using BooksMiddletier.OtherClasses;
using BooksMiddletier.Requests;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BooksApi.Controllers
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private NotificationHubClient hub;

        public NotificationsController()
        {
            hub = Notifications.Instance.Hub;
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string PushChannel { get; set; }
            public List<string> Tags { get; set; }
            public Guid InstalationId { get; set; }

        }

        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        public async Task<HttpResponseMessage> Post(Installation deviceUpdate)
        {
            try
            {
                RegistrationDescription registration = null;
                switch (deviceUpdate.Platform)
                {
                    case NotificationPlatform.Mpns:
                        registration = new MpnsRegistrationDescription(deviceUpdate.PushChannel);
                        break;
                    case NotificationPlatform.Wns:
                        registration = new WindowsRegistrationDescription(deviceUpdate.PushChannel);
                        break;
                    case NotificationPlatform.Apns:
                        registration = new AppleRegistrationDescription(deviceUpdate.PushChannel);
                        break;
                    case NotificationPlatform.Gcm:
                        registration = new GcmRegistrationDescription(deviceUpdate.PushChannel);
                        break;
                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                await hub.CreateOrUpdateInstallationAsync(deviceUpdate);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[NotificationRegistration] There was an error registering device for notifications. Request: {deviceUpdate}");
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            try
            {
                await hub.DeleteInstallationAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[NotificationUnregister] There was an error unregistering device from notifications. id: {id}");
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }

    }
}
