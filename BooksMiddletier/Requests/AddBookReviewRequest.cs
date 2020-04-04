using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class AddBookReviewRequest
    {
        public int ISBN { get; set; }
        public Guid ReviewerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
