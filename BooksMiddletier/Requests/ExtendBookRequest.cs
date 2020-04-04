using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class ExtendBookRequest
    {
        public Guid OfferId { get; set; }
        public DateTime ProposedDate { get; set; }
    }
}
