using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class TestResponse : DefaultResponse
    {
        public List<Result> Results { get; set; }
    }
}
