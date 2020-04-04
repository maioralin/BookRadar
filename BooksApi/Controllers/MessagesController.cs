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
using static BooksApi.DbHelper;

namespace BooksApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        [Route("save")]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [HttpPost]
        public DefaultResponse Save([FromBody]SaveMessageRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("MessageFrom", System.Data.ParameterDirection.Input, request.From));
                parameters.Add(new DbParameter("MessageTo", System.Data.ParameterDirection.Input, request.To));
                parameters.Add(new DbParameter("MessageContent", System.Data.ParameterDirection.Input, request.Message));
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                var result = db.ExecuteNonQuery("spBooks_AddMessage", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[Save] There was an error saving message. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [Route("getByRequestId")]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [HttpGet]
        public GetMessagesResponse GetByRequestId(int requestId, DateTime startDate)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("StartDate", System.Data.ParameterDirection.Input, startDate));
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, requestId));
                var result = db.ExecuteList<MessageSQL>("spBooks_GetMessages", parameters);
                return new GetMessagesResponse
                {
                    Messages = result,
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetByRequestId] Error getting messages for requestid {requestId}");
                return new GetMessagesResponse
                {
                    Messages = null,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }
    }
}
