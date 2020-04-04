using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class RequestsResponse: DefaultResponse
    {
        public List<RequestMinInfo> Requests { get; set; }
    }
}
