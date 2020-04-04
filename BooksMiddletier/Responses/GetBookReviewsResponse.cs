using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class GetBookReviewsResponse: DefaultResponse
    {
        public List<BookReviewSQL> Reviews { get; set; }
        public int Total { get; set; }
        public decimal Average { get; set; }
    }
}
