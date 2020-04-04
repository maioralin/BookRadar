using BooksMiddletier.OtherClasses;
using BooksMiddletier.Requests;
using BooksMiddletier.Responses;
using BooksMiddletier.SqlClasses;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static BooksApi.DbHelper;

namespace BooksApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/notificationSender")]
    public class NotificationSenderController : ApiController
    {
        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("send")]
        public async Task<DefaultResponse> Post([FromBody]NotificationRequest request)
        {
            try
            {
                QueryString queryString = new QueryString();
                foreach (var p in request.Params)
                {
                    queryString.Add(p.Key, p.Value);
                }

                ToastContent toast = new ToastContent()
                {
                    Launch = queryString.ToString(),
                    Visual = new ToastVisual
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                        {
                            new AdaptiveText()
                            {
                                Text = request.Title,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = request.Message
                            }
                        },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = "https://s10.postimg.org/tc1cwons9/books.jpg",
                                HintCrop = ToastGenericAppLogoCrop.Circle
                            }
                        }
                    }
                };
                await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast.GetContent(), request.Target);

                var data = new JObject();
                data.Add("message", request.Message);
                data.Add("title", request.Title);
                foreach (var p in request.Params)
                {
                    data.Add(p.Key, p.Value);
                }

                var obj = new JObject();
                obj.Add("data", data);

                var json = obj.ToString();

                await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(json, request.Target);

                data = new JObject();
                var aps = new JObject();
                var alert = new JObject();
                alert.Add("body", request.Message);
                alert.Add("title", request.Title);
                aps.Add("alert", alert);
                string action = request.Params["action"];
                string requestId = string.Empty;
                try
                {
                    requestId = request.Params["requestId"];
                }
                catch(Exception e)
                {

                }
                data.Add("aps", aps);
                data.Add("action", action);
                data.Add("requestId", !string.IsNullOrEmpty(requestId) ? requestId : "0");
                json = data.ToString();

                await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(json, request.Target);

                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };

            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[NotificationSend] There was an error sending notification. Request: {request}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("remember")]
        public async Task<DefaultResponse> Remember()
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                DateTime date = DateTime.UtcNow;
                parameters.Add(new DbParameter("CurrentDate", System.Data.ParameterDirection.Input, date));
                var result = db.ExecuteList<ReminderSQL>("spBooks_GetNotificationReminders", parameters);

                foreach (var reminder in result)
                {
                    SendReminderNotification(reminder);
                }

                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[Remember] There was an error sending remember notifications.");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("sendErrorEmail")]
        public async Task<DefaultResponse> SendErrorEmail([FromBody]EmailRequest request)
        {
            using (SmtpClient smtp = new SmtpClient("smtp.zoho.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("noreply@mobxsoft.com", "");
                using (MailMessage msg = new MailMessage("noreply@mobxsoft.com", ""))
                {
                    msg.Subject = "In App Error!";
                    msg.Body = $"[{ DateTime.UtcNow.ToString()}] - { "Error"} - { "In App Error!"}\r\n{ request.Exception.Message.ToString() }";

                    smtp.Send(msg);
                }
            }
            return new DefaultResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };
        }

        private static async void SendReminderNotification(ReminderSQL reminder)
        {
            QueryString queryString = new QueryString();
            queryString.Add("requestId", reminder.RequesterId.ToString());

            ToastContent toast = new ToastContent()
            {
                Launch = queryString.ToString(),
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Return book reminder",
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = $"Your book, {reminder.Title}, from {reminder.Owner} is due tomorrow. Please return the book, or ask the owner for more time, if you haven't finished it yet."
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "https://s10.postimg.org/tc1cwons9/books.jpg",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                }
            };
            await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast.GetContent(), reminder.RequesterId.ToString());

            var data = new JObject();
            data.Add("message", $"Your book, {reminder.Title}, from {reminder.Owner} is due tomorrow. Please return the book, or ask the owner for more time, if you haven't finished it yet.");
            data.Add("title", "Return book reminder");
            data.Add("requestId", reminder.Id);

            var obj = new JObject();
            obj.Add("data", data);

            var json = obj.ToString();

            await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(json, reminder.RequesterId.ToString());
        }
    }
}
