using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class RequestBookRequest
    {
        public Guid UserId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid BookId { get; set; }
        public bool Wanted { get; set; }
    }
}
