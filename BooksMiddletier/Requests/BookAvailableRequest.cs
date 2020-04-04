using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class BookAvailableRequest
    {
        public Guid BookId { get; set; }
        public bool Available { get; set; }
    }
}
