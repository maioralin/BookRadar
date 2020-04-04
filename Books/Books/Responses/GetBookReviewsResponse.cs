using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class GetBookReviewsResponse : DefaultResponse
    {
        public List<BookReviewSQL> Reviews { get; set; }
        public int Total { get; set; }
        public decimal Average { get; set; }
    }
}
