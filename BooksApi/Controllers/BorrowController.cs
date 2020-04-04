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
    [RoutePrefix("api/borrow")]
    public class BorrowController : ApiController
    {
        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("requestBook")]
        public DefaultResponse RequestBook([FromBody]RequestBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("Requests", System.Data.ParameterDirection.Output, 0));
                parameters.Add(new DbParameter("Books", System.Data.ParameterDirection.Output, 1));
                var result = db.ExecuteNonQuery("spBooks_GetUserStats", parameters);
                int requests = (int)db.OutParameters[0].Value;
                int books = (int)db.OutParameters[1].Value;
                if (requests >= books * 5)
                {
                    return new DefaultResponse
                    {
                        ErrorCode = 1,
                        ErrorMessage = "Number of book request cannot exceed 5 times the number of books in your library. Please add more books in your library, or remove books from your wishlist"
                    };
                }
                parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                parameters.Add(new DbParameter("OwnerId", System.Data.ParameterDirection.Input, request.OwnerId));
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.BookId));
                parameters.Add(new DbParameter("Wanted", System.Data.ParameterDirection.Input, request.Wanted));
                result = db.ExecuteNonQuery("spBooks_AddBookRequest", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[RequestBook] There was an error requesting a book. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("offerBook")]
        public GuidResponse OfferBook([FromBody]OfferBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                parameters.Add(new DbParameter("ProposedReturnDate", System.Data.ParameterDirection.Input, request.ProposedReturnDate));
                var result = db.ExecuteSingle<GuidIdentity>("spBooks_AddOffer", parameters);
                return new GuidResponse
                {
                    Id = result.Id,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[OfferBook] There was an error offering a book. Request: {request.ToString()}");
                return new GuidResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("requestTime")]
        public DefaultResponse RequestTime([FromBody]ExtendBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("OfferId", System.Data.ParameterDirection.Input, request.OfferId));
                parameters.Add(new DbParameter("ProposedReturnDate", System.Data.ParameterDirection.Input, request.ProposedDate));
                var result = db.ExecuteNonQuery("spBooks_ExtendTimeRequest", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[RequestTime] There was an error requesting extra time. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("requestTimeResponse")]
        public DefaultResponse RequestTimeResponse([FromBody]ExtendBookResponseRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                if (request.Answer)
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("OfferId", System.Data.ParameterDirection.Input, request.OfferId));
                    parameters.Add(new DbParameter("ProposedDate", System.Data.ParameterDirection.Input, request.ProposedDate));
                    var result = db.ExecuteNonQuery("spBooks_AcceptTimeRequest", parameters);
                }
                else
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("OfferId", System.Data.ParameterDirection.Input, request.OfferId));
                    var result = db.ExecuteNonQuery("spBooks_RejectTimeRequest", parameters);
                }
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[RequestTimeResponse] There was an error processing request. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("deleteOffer")]
        public DefaultResponse DeleteOffer([FromBody]AcceptBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                var result = db.ExecuteNonQuery("spBooks_DeleteOffer", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[DeleteOffer] There was an error deleting offer. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("acceptBook")]
        public DefaultResponse AcceptBook([FromBody]AcceptBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                DateTime acceptDate = DateTime.UtcNow;
                parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.BookId));
                parameters.Add(new DbParameter("RequesterId", System.Data.ParameterDirection.Input, request.RequesterId));
                var result = db.ExecuteSingle<AcceptBookSQL>("spBooks_AcceptOffer", parameters);
                parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BorrowerId", System.Data.ParameterDirection.Input, result.RequesterId));
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, result.BookId));
                parameters.Add(new DbParameter("AcceptedDate", System.Data.ParameterDirection.Input, acceptDate));
                db.ExecuteSingle<AcceptBookSQL>("spBooks_AddBorrowingHistory", parameters);
                if (result.Donate || result.Giveaway)
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("BookRequestId", System.Data.ParameterDirection.Input, result.RequestId));
                    parameters.Add(new DbParameter("RequesterId", System.Data.ParameterDirection.Input, result.RequesterId));
                    parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, result.BookId));
                    var result2 = db.ExecuteNonQuery("spBooks_DonateBook", parameters);
                }
                if (result.PermanentExchange.HasValue && result.PermanentExchange.Value)
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("RequesterId", System.Data.ParameterDirection.Input, result.RequesterId));
                    parameters.Add(new DbParameter("OwnerId", System.Data.ParameterDirection.Input, result.OwnerId));
                    parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, result.BookId));
                    parameters.Add(new DbParameter("ProposedBookId", System.Data.ParameterDirection.Input, result.ProposedBookId));
                    parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                    var result2 = db.ExecuteNonQuery("spBooks_PermanentExchange", parameters);
                }
                if (result.ProposedBookId.HasValue && (result.PermanentExchange.HasValue && !result.PermanentExchange.Value))
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, result.ProposedBookId));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, result.OwnerId));
                    var result2 = db.ExecuteNonQuery("spBooks_UpdateExchangeBook", parameters);
                }
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AcceptBook] There was an error accepting a book. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("returnBook")]
        public DefaultResponse ReturnBook([FromBody]AcceptBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                var result = db.ExecuteNonQuery("spBooks_ReturnBook", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[ReturnBook] There was an error returning book. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("deleteReturn")]
        public DefaultResponse DeleteReturn([FromBody]AcceptBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                var result = db.ExecuteNonQuery("spBooks_DeleteReturn", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[DeleteReturn] There was an error processing request. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("acceptReturnBook")]
        public DefaultResponse AcceptReturnBook([FromBody]AcceptBookRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookOfferId", System.Data.ParameterDirection.Input, request.BookOfferId));
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.BookId));
                parameters.Add(new DbParameter("RequesterId", System.Data.ParameterDirection.Input, request.RequesterId));
                parameters.Add(new DbParameter("ActualReturnDate", System.Data.ParameterDirection.Input, DateTime.UtcNow));
                var result = db.ExecuteNonQuery("spBooks_AcceptReturnBook", parameters);
                if (request.ProposedReturnBook.HasValue)
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.ProposedReturnBook.Value));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, null));
                    var result2 = db.ExecuteNonQuery("spBooks_UpdateExchangeBook", parameters);
                }
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[AcceptReturnBook] There was an error processing request. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("bookExchange")]
        public DefaultResponse BookExchange([FromBody]BookExchangeRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.OfferedBookId));
                parameters.Add(new DbParameter("PermanentExchange", System.Data.ParameterDirection.Input, request.PermanentExchange));
                parameters.Add(new DbParameter("AcceptedExchange", System.Data.ParameterDirection.Input, request.AcceptedExchange));
                var result = db.ExecuteNonQuery("spBooks_OfferBookExchange", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[BookExchange] There was an error processing request. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("rejectBookExchange")]
        public DefaultResponse RejectBookExchange([FromBody]BookExchangeRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                var result = db.ExecuteNonQuery("spBooks_RejectBookExchange", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[RejectBookExchange] There was an error rejecting book exchange. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("makeBookAvailable")]
        public DefaultResponse MakeBookAvailable([FromBody]BookAvailableRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, request.BookId));
                parameters.Add(new DbParameter("Available", System.Data.ParameterDirection.Input, request.BookId));
                var result = db.ExecuteNonQuery("spBooks_SetBookAvailability", parameters);
                return new GuidResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[MakeBookAvailable] There was an error making a book available. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBooksRequestedFromMe")]
        public RequestsResponse GetBooksRequestedFromMe(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<RequestMinInfo>("spBooks_GetBooksRequestedFromMe", parameters);
                return new RequestsResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "",
                    Requests = result
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBooksRequestedFromMe] There was an error getting books requested from user. userId: {userId}, pageNumber: {pageNumber}, pageSize: {pageSize}");
                return new RequestsResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error",
                    Requests = null
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBooksRequestedByMe")]
        public RequestsResponse GetBooksRequestedByMe(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<RequestMinInfo>("spBooks_GetBooksRequestedByMe", parameters);
                return new RequestsResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "",
                    Requests = result
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBooksRequestedByMe] There was an error getting books requested by user. userId: {userId}, pageNumber: {pageNumber}, pageSize: {pageSize}");
                return new RequestsResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error",
                    Requests = null
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getRequest")]
        public BookRequestResponse GetRequest(int requestId)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, requestId));
                var result = db.ExecuteSingle<BookRequestSQL>("spBooks_GetBookRequest", parameters);
                BookRequest bookRequest = new BookRequest
                {
                    Id = requestId,
                    OwnerId = result.OwnerId,
                    OwnerName = result.OwnerName,
                    RequesterId = result.RequesterId,
                    RequesterName = result.RequesterName,
                    Donate = result.Donate,
                    BookId = result.BookId,
                    Giveaway = result.Giveaway,
                    ProposedBookId = result.ProposedBookId,
                    AcceptedExchange = result.AcceptedExchange ?? false,
                    PermanentExchange = result.PermanentExchange ?? false,
                    Authors = result.Authors,
                    Cover = result.Cover,
                    Title = result.Title,
                    BookOffer = new BookOffer
                    {
                        Id = result.OfferId,
                        ActualReturnDate = result.ActualReturnDate,
                        ProposedReturnDate = result.ProposedReturnDate,
                        BookRequestId = requestId,
                        BookAccepted = result.BookAccepted,
                        ReturnOffered = result.ReturnOffered,
                        ExtendedDate = result.ExtendedDate
                    }
                };
                if (bookRequest.ProposedBookId.HasValue)
                {
                    parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, bookRequest.ProposedBookId.Value));
                    var result2 = db.ExecuteSingle<BookDetailsSQL>("spBooks_GetBookById", parameters);
                    BooksMiddletier.OtherClasses.Book b = new BooksMiddletier.OtherClasses.Book();
                    b.ISBN = result2.ISBN;
                    b.Title = result2.Title;
                    b.MediumCover = result2.MediumCover;
                    b.Owner = new OwnerDetails
                    {
                        Name = result2.Name,
                        UserId = result2.UserId
                    };
                    b.BorrowerId = result2.BorrowerId;
                    b.BorrowerName = result2.BorrowerName;
                    b.Authors = result2.AuthorName;
                    bookRequest.ProposedBook = b;
                }
                bool isReviewed = false;
                parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, requestId));
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, bookRequest.RequesterId));
                parameters.Add(new DbParameter("IsReviewed", System.Data.ParameterDirection.Output, isReviewed));
                db.ExecuteNonQuery("spBooks_IsUserReviewed", parameters);
                bookRequest.IsUserReviewed = (bool)db.OutParameters[0].Value;
                return new BookRequestResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "",
                    BookRequest = bookRequest
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetRequest] There was an error getting a book request. requestId: {requestId}");
                return new BookRequestResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error",
                    BookRequest = null
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("closeRequest")]
        public DefaultResponse CloseRequest([FromBody]CloseChatRequest request)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("RequestId", System.Data.ParameterDirection.Input, request.RequestId));
                db.ExecuteNonQuery("spBooks_CloseRequest", parameters);
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"There was an error closing a book request. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }
    }
}
