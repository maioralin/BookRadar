using BooksMiddletier.OtherClasses;
using BooksMiddletier.Requests;
using BooksMiddletier.Responses;
using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using static BooksApi.DbHelper;

namespace BooksApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/facebook")]
    public class FacebookAccountController : ApiController
    {
        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("login")]
        public FacebookLoginResponse Login([FromBody]FacebookLoginRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("Id", System.Data.ParameterDirection.Input, request.ID));
                parameters.Add(new DbParameter("Email", System.Data.ParameterDirection.Input, request.Email));
                parameters.Add(new DbParameter("LastName", System.Data.ParameterDirection.Input, request.LastName));
                parameters.Add(new DbParameter("Picture", System.Data.ParameterDirection.Input, request.Picture));
                parameters.Add(new DbParameter("Gender", System.Data.ParameterDirection.Input, request.Gender));
                parameters.Add(new DbParameter("Result", System.Data.ParameterDirection.Output, 0));
                parameters.Add(new DbParameter("NewUser", System.Data.ParameterDirection.Output, false));
                var result = db.ExecuteNonQuery("spBooks_LoginRegister", parameters);
                var outResult = db.OutParameters[0];
                bool newUser = (bool)db.OutParameters[1].Value;
                if (newUser)
                {
                    using (SmtpClient smtp = new SmtpClient("smtp.zoho.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("noreply@mobxsoft.com", "");
                        using (MailMessage msg = new MailMessage("noreply@mobxsoft.com", request.Email))
                        {
                            msg.Subject = "Welcome to Social Books";
                            msg.Body = "message body";

                            smtp.Send(msg);
                        }
                    }

                }
                return new FacebookLoginResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error($"[Login] There was an error logging with Facebook. Request: {request.ToString()}");
                return new FacebookLoginResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Authorize]
        [Route("getUserDetailsByFacebookId")]
        public UserDetailsResponse GetUserDetailsByFacebookId(string facebookId)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("Id", System.Data.ParameterDirection.Input, facebookId));
                var result = db.ExecuteSingle<UserDetails>("spBooks_GetUserDetailsByFacebookId", parameters);
                return new UserDetailsResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "",
                    Info = result
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetUserDetailsByFacebookId] There was an error getting user details by Facebook id. facebookId: {facebookId}");
                return new UserDetailsResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "",
                    Info = null
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("updateUserLocation")]
        public string UpdateUserLocation([FromBody]UpdateUserLocationRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("Latitude", System.Data.ParameterDirection.Input, request.Latitude));
                parameters.Add(new DbParameter("Longitude", System.Data.ParameterDirection.Input, request.Longitude));
                var result = db.ExecuteSingle<UserDetails>("spBooks_UpdateUserLocation", parameters);
                return result.ToString();
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[UpdateUserLocation] There was an error updating user location. Request: {request.ToString()}");
                return string.Empty;
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("addReferalCode")]
        public DefaultResponse AddReferalCodeA([FromBody]ReferalCodeRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                bool boolResult = false;
                parameters.Add(new DbParameter("Code", System.Data.ParameterDirection.Input, request.Code));
                parameters.Add(new DbParameter("Exists", System.Data.ParameterDirection.Output, boolResult));
                var result = db.ExecuteNonQuery("spBooks_CheckReferralCode", parameters);
                bool r = (bool)db.OutParameters[0].Value;
                if (r)
                {
                    return new DefaultResponse
                    {
                        ErrorCode = 1,
                        ErrorMessage = "Referral code already taken"
                    };
                }
                else
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("Code", System.Data.ParameterDirection.Input, request.Code));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                    result = db.ExecuteNonQuery("spBooks_AddReferralCode", parameters);
                    return new DefaultResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = string.Empty
                    };
                }
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AddReferalCodeA] There was an error adding a new referal code. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("addInviteCode")]
        public DefaultResponse AddInviteCode([FromBody]ReferalCodeRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                bool boolResult = false;
                parameters.Add(new DbParameter("Code", System.Data.ParameterDirection.Input, request.Code));
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("Exists", System.Data.ParameterDirection.Output, boolResult));
                var result = db.ExecuteNonQuery("spBooks_CheckInviteCode", parameters);
                bool r = (bool)db.OutParameters[0].Value;
                if (!r)
                {
                    return new DefaultResponse
                    {
                        ErrorCode = 1,
                        ErrorMessage = "Invite code invalid"
                    };
                }
                else
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("Code", System.Data.ParameterDirection.Input, request.Code));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                    result = db.ExecuteNonQuery("spBooks_AddInviteCode", parameters);
                    return new DefaultResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = string.Empty
                    };
                }
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AddInviteCode] There was an error adding a new invite code. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("purchaseAdFree")]
        public DefaultResponse PurchaseAdFree([FromBody]PurchaseAdFreeRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("PurchaseToken", System.Data.ParameterDirection.Input, request.PurchaseToken));
                parameters.Add(new DbParameter("PurchaseId", System.Data.ParameterDirection.Input, request.PurchaseId));
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                var result = db.ExecuteNonQuery("spBooks_PurchaseAdFree", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[PurchaseAdFree] There was an error saving the purchase details. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getDetails")]
        public UserResponse GetDetails(Guid userId)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                var result = db.ExecuteSingle<User>("spBooks_GetUserDetails", parameters);

                return new UserResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty,
                    User = result
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetDetails] There was an error getting user details. guid: {userId}");
                return new UserResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error",
                    User = null
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("delete")]
        public DefaultResponse Delete([FromBody] DeleteAccountRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                bool hasUnreturnedBooks = false;
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("HasUnreturnedBooks", System.Data.ParameterDirection.Output, hasUnreturnedBooks));
                var result = db.ExecuteNonQuery("spBooks_HasUnreturnedBooks", parameters);
                hasUnreturnedBooks = (bool)db.OutParameters[0].Value;
                if(hasUnreturnedBooks)
                {
                    return new DefaultResponse
                    {
                        ErrorCode = 1,
                        ErrorMessage = "User has unreturned books"
                    };
                }
                else
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                    result = db.ExecuteNonQuery("spBooks_DeleteUser", parameters);
                    return new DefaultResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = string.Empty
                    };
                }
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[DeleteUser] There was an error deleting user. guid: {request.UserId}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }
    }
}
