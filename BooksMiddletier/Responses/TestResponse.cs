using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class TestResponse : DefaultResponse
    {
        public List<Result> Results { get; set; }
    }
}
