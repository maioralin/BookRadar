using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class UserDetailsRequest
    {
        public Guid RequesterId { get; set; }
        public Guid UserId { get; set; }
    }
}
