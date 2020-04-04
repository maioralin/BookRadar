using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class BookSearchByDistanceResponse : DefaultResponse
    {
        public List<BookMinInfo> Books { get; set; }
    }
}
