using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class BookSearchByDistanceResponse : DefaultResponse
    {
        public List<BookMinInfo> Books { get; set; }
    }
}
