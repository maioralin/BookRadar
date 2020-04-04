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
    [RoutePrefix("api/reviews")]
    public class ReviewsController : ApiController
    {
        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("addBookReview")]
        public DefaultResponse Add([FromBody]AddBookReviewRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("ISBN", System.Data.ParameterDirection.Input, request.ISBN));
                parameters.Add(new DbParameter("ReviewerId", System.Data.ParameterDirection.Input, request.ReviewerId));
                parameters.Add(new DbParameter("Rating", System.Data.ParameterDirection.Input, request.Rating));
                parameters.Add(new DbParameter("Comment", System.Data.ParameterDirection.Input, request.Comment));
                var result = db.ExecuteNonQuery("spBooks_AddBookReview", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AddBookReview] There was an error adding a book review. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("hasReviewAdded")]
        public BooleanResponse HasReviewAdded(Guid userId, string ISBN)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                bool boolResult = true;
                parameters.Add(new DbParameter("ISBN", System.Data.ParameterDirection.Input, ISBN));
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("Reviewed", System.Data.ParameterDirection.Output, boolResult));
                var result = db.ExecuteNonQuery("spBooks_UserReviewedBook", parameters);
                bool r = (bool)db.OutParameters[0].Value;
                return new BooleanResponse
                {
                    Result = r,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[HasReviewAdded] There was an error processing request. userId: {userId}, ISBN: {ISBN}");
                return new BooleanResponse
                {
                    Result = false,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("addUserReview")]
        public DefaultResponse AddUserReview([FromBody]AddUserReviewRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("ReviewerId", System.Data.ParameterDirection.Input, request.ReviewerId));
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                parameters.Add(new DbParameter("BookQuality", System.Data.ParameterDirection.Input, request.BookAspect));
                parameters.Add(new DbParameter("TimeQuality", System.Data.ParameterDirection.Input, request.ReturnTime));
                parameters.Add(new DbParameter("Comment", System.Data.ParameterDirection.Input, request.Comment));
                var result = db.ExecuteNonQuery("spBooks_AddUserReview", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AddUserReview] There was an error adding a user review. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBookReviews")]
        public GetBookReviewsResponse GetBookReviews(int isbn, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("ISBN", System.Data.ParameterDirection.Input, isbn));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                parameters.Add(new DbParameter("Total", System.Data.ParameterDirection.Output, 0));
                parameters.Add(new DbParameter("Average", System.Data.ParameterDirection.Output, 0.0));
                var result = db.ExecuteList<BookReviewSQL>("spBooks_GetBookReviews", parameters);
                int total = (int)db.OutParameters[0].Value;
                decimal average = decimal.Parse(db.OutParameters[1].Value.ToString());
                return new GetBookReviewsResponse
                {
                    Reviews = result,
                    Total = total,
                    Average = average,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBookReviews] There was an erro get reviews for book with ISBN {isbn}, pageNumber {pageNumber}, pageSize: {pageSize}");
                return new GetBookReviewsResponse
                {
                    Reviews = null,
                    Total = 0,
                    Average = 0,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getUserReviews")]
        public UserReviewsResponse GetUserReviews(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<UserReview>("spBooks_GetUserReviews", parameters);
                return new UserReviewsResponse
                {
                    Reviews = result,
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetUserReviews] There was an error getting reviews for user with id {userId}, pageNumber {pageNumber}, pagesSize {pageSize}");
                return new UserReviewsResponse
                {
                    Reviews = null,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }
    }
}
