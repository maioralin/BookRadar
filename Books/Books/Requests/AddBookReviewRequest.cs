using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class AddBookReviewRequest
    {
        public int ISBN { get; set; }
        public Guid ReviewerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
