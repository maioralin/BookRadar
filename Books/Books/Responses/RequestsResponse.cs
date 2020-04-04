using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class RequestsResponse: DefaultResponse
    {
        public List<RequestMinInfo> Requests { get; set; }
    }
}
