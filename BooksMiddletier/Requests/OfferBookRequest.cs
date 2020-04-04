using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class OfferBookRequest
    {
        public int RequestId { get; set; }
        public DateTime ProposedReturnDate { get; set; }
    }
}
