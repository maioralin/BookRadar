using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class ReferalCodeRequest
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
