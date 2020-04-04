using BooksMiddletier.ISBNDB;
using BooksMiddletier.OtherClasses;
using BooksMiddletier.Requests;
using BooksMiddletier.Responses;
using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static BooksApi.DbHelper;

namespace BooksApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBooksByUserId")]
        public UserBooksResponse GetBooksByUserId(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<SqlBook>("spBooks_GetUserBooks", parameters);
                List<BooksMiddletier.OtherClasses.Book> books = new List<BooksMiddletier.OtherClasses.Book>();
                foreach (var v in result)
                {
                    BooksMiddletier.OtherClasses.Book b = new BooksMiddletier.OtherClasses.Book();
                    b.Id = v.BookId;
                    b.ISBN = v.ISBN;
                    b.Title = v.Title;
                    b.Authors = v.AuthorName;
                    b.SmallCover = v.Cover;
                    books.Add(b);
                }

                return new UserBooksResponse
                {
                    Books = books,
                    TotalRows = result != null && result.Count > 0 ? result.FirstOrDefault().TotalRows : 0,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBooksByUserId] There was an error getting user books by id. userId: {userId}, pageNumber: {pageNumber}, pageSize: {pageSize}");
                return new UserBooksResponse
                {
                    Books = null,
                    TotalRows = 0,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBookById")]
        public BookByIdResponse GetBookById(Guid bookId)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("BookId", System.Data.ParameterDirection.Input, bookId));
                var result = db.ExecuteSingle<BookDetailsSQL>("spBooks_GetBookById", parameters);
                BooksMiddletier.OtherClasses.Book b = new BooksMiddletier.OtherClasses.Book();
                b.ISBN = result.ISBN;
                b.Title = result.Title;
                b.MediumCover = result.MediumCover;
                b.Owner = new OwnerDetails
                {
                    Name = result.Name,
                    UserId = result.UserId
                };
                b.BorrowerId = result.BorrowerId;
                b.BorrowerName = result.BorrowerName;
                b.Authors = result.AuthorName;
                b.Donate = result.Donate;
                b.Giveaway = result.Giveaway;
                b.Description = result.Description;

                return new BookByIdResponse
                {
                    Book = b,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBookById] There was an error getting book by id. bookId: {bookId}");
                return new BookByIdResponse
                {
                    Book = null,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBooksByDistance")]
        public BookSearchByDistanceResponse GetBooksByDistance(Guid userId, int maxDistance, decimal latitude, decimal longitude, int pageNumber, int pageSize)
        {
            GlobalVars.Logger.Information($"[GetBooksByDistance] Request made: UserId {userId}, MaxDistance: {maxDistance}, Latitude: {latitude}, Longitude: {longitude}, PageNumber: {pageNumber}, PageSize: {pageSize}");
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("Latitude", System.Data.ParameterDirection.Input, latitude));
                parameters.Add(new DbParameter("Longitude", System.Data.ParameterDirection.Input, longitude));
                parameters.Add(new DbParameter("MaxDistance", System.Data.ParameterDirection.Input, maxDistance));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<BookMinInfo>("spBooks_GetBooksByDistance", parameters);
                return new BookSearchByDistanceResponse
                {
                    Books = result,
                    ErrorCode = 0,
                    ErrorMessage = "Internal Server Error"
                };
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBooksByDistance] There was an error getting books by distance. userId: {userId}, maxDistance: {maxDistance}, latitude: {latitude}, longitude: {longitude}, pageNumber: {pageNumber}, pageSize {pageSize}");
                return new BookSearchByDistanceResponse
                {
                    Books = null,
                    ErrorCode = 500,
                    ErrorMessage = e.ToString()
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("getBorrowHistory")]
        public BorrowHistoryResponse GetBorrowHistory(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, userId));
                parameters.Add(new DbParameter("PageNumber", System.Data.ParameterDirection.Input, pageNumber));
                parameters.Add(new DbParameter("PageSize", System.Data.ParameterDirection.Input, pageSize));
                var result = db.ExecuteList<BorrowHistorySQL>("spBooks_GetBorrowingHistory", parameters);
                return new BorrowHistoryResponse
                {
                    BorrowedBooks = result,
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch (Exception e)
            {
                GlobalVars.Logger.Error(e, $"[GetBorrowHistory] There was an error getting borrow history. userId: {userId}, pageNumber: {pageNumber}, pageSize: {pageSize}");
                return new BorrowHistoryResponse
                {
                    BorrowedBooks = null,
                    ErrorCode = 500,
                    ErrorMessage = e.ToString()
                };
            }
        }

        [HttpPost]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("add")]
        public DefaultResponse Add([FromBody]AddBookRequest request)
        {
            //add authors, get author ids
            //add book, insert author ids as nvarchar separated by commas
            try
            {
                if (!request.OwnBook)
                {
                    DbHelper db = new DbHelper();
                    List<DbParameter> parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("GoodreadsId", System.Data.ParameterDirection.Input, int.Parse(request.Book.GoodreadsId)));
                    parameters.Add(new DbParameter("WorkId", System.Data.ParameterDirection.Input, int.Parse(request.Book.WorkId)));
                    parameters.Add(new DbParameter("AuthorName", System.Data.ParameterDirection.Input, request.Book.AuthorName));
                    parameters.Add(new DbParameter("Title", System.Data.ParameterDirection.Input, request.Book.Title));
                    parameters.Add(new DbParameter("Cover", System.Data.ParameterDirection.Input, request.Book.LargeCover));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                    parameters.Add(new DbParameter("Exchange", System.Data.ParameterDirection.Input, request.Exchange));
                    parameters.Add(new DbParameter("Giveaway", System.Data.ParameterDirection.Input, request.Giveaway));
                    parameters.Add(new DbParameter("Description", System.Data.ParameterDirection.Input, request.Description));
                    var result = db.ExecuteNonQuery("spBooks_AddBook", parameters);
                }
                else
                {
                    DbHelper db = new DbHelper();
                    List<DbParameter> parameters = new List<DbParameter>();
                    parameters.Add(new DbParameter("AuthorName", System.Data.ParameterDirection.Input, request.Book.Authors));
                    parameters.Add(new DbParameter("Title", System.Data.ParameterDirection.Input, request.Book.Title));
                    parameters.Add(new DbParameter("Cover", System.Data.ParameterDirection.Input, "https://s.gr-assets.com/assets/nophoto/book/111x148-bcc042a9c91a29c1d680899eff700a03.png"));
                    parameters.Add(new DbParameter("UserId", System.Data.ParameterDirection.Input, request.UserId));
                    parameters.Add(new DbParameter("Exchange", System.Data.ParameterDirection.Input, request.Exchange));
                    parameters.Add(new DbParameter("Giveaway", System.Data.ParameterDirection.Input, request.Giveaway));
                    parameters.Add(new DbParameter("Description", System.Data.ParameterDirection.Input, request.Description));
                    var result = db.ExecuteNonQuery("spBooks_AddOwnBook", parameters);
                }
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = ""
                };
            }
            catch(Exception e)
            {

                GlobalVars.Logger.Error(e, $"[Books/Add] There was an error adding a new book. Request: {request.ToString()}");
                return new DefaultResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }

        [HttpGet]
        [ThrottleFilter(ThrottleGroup: "ipaddress")]
        [Route("searchByISBN")]
        public async Task<SearchByISBNResponse> SearchByISBN(string isbn)
        {
            try
            {
                var resp = new SearchByISBNResponse
                {
                    Found = false
                };
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("ISBN", System.Data.ParameterDirection.Input, isbn));
                var result = db.ExecuteSingle<BookMinInfoByISBN>("spBooks_GetBookDetailsByISBN", parameters);
                if (result.ISBN13 == null)
                {
                    db = new DbHelper();
                    var response = await RequestHelper.MakeGetRequest<BookByISBN>($"book/{isbn}");
                    if (response != null)
                    {
                        parameters = new List<DbParameter>();
                        parameters.Add(new DbParameter("ISBN13", System.Data.ParameterDirection.Input, response.book.Isbn13));
                        parameters.Add(new DbParameter("ISBN", System.Data.ParameterDirection.Input, response.book.Isbn));
                        parameters.Add(new DbParameter("FORMAT", System.Data.ParameterDirection.Input, response.book.Format));
                        parameters.Add(new DbParameter("IMAGE", System.Data.ParameterDirection.Input, response.book.Image ?? "https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png"));
                        parameters.Add(new DbParameter("TITLE_LONG", System.Data.ParameterDirection.Input, response.book.Title_long));
                        parameters.Add(new DbParameter("DATE_PUBLISHED", System.Data.ParameterDirection.Input, response.book.Date_published));
                        parameters.Add(new DbParameter("TITLE", System.Data.ParameterDirection.Input, response.book.Title));
                        parameters.Add(new DbParameter("PUBLISHER", System.Data.ParameterDirection.Input, response.book.Publisher));
                        parameters.Add(new DbParameter("LANGUAGE", System.Data.ParameterDirection.Input, response.book.Language));
                        parameters.Add(new DbParameter("OVERVIEW", System.Data.ParameterDirection.Input, response.book.Overview));
                        parameters.Add(new DbParameter("DIMENSIONS", System.Data.ParameterDirection.Input, response.book.Dimensions));
                        parameters.Add(new DbParameter("DEWEY_DECIMAL", System.Data.ParameterDirection.Input, response.book.Dewey_decimal));
                        parameters.Add(new DbParameter("EDITION", System.Data.ParameterDirection.Input, response.book.Edition));
                        parameters.Add(new DbParameter("AUTHORS", System.Data.ParameterDirection.Input, string.Join(",", response.book.Authors)));
                        parameters.Add(new DbParameter("SUBJECTS", System.Data.ParameterDirection.Input, string.Join(",", response.book.Subjects)));
                        db.ExecuteNonQuery("spBooks_AddBookDetails", parameters);
                        BookMinInfoByISBN b = new BookMinInfoByISBN
                        {
                            Authors = string.Join(",", response.book.Authors),
                            Image = response.book.Image,
                            ISBN13 = response.book.Isbn13,
                            Title = response.book.Title
                        };
                        resp.Found = true;
                        resp.Book = b;
                        resp.ErrorCode = 0;
                        resp.ErrorMessage = "";
                    }
                    else
                    {
                        resp.ErrorCode = 1;
                        resp.ErrorMessage = "Book not found";
                        resp.Book = null;
                    }
                }
                else
                {
                    resp.Found = true;
                    resp.Book = result;
                    resp.ErrorCode = 0;
                    resp.ErrorMessage = "";
                }
                return resp;
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[SearchByISBN] There was an error searching by ISBN. ISBN: {isbn}");
                return new SearchByISBNResponse
                {
                    Book = null,
                    Found = false,
                    ErrorCode = 500,
                    ErrorMessage = "Internal Server Error"
                };
            }
        }
    }
}
