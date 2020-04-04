using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class ExtendBookResponseRequest
    {
        public Guid OfferId { get; set; }
        public bool Answer { get; set; }
        public DateTime ProposedDate { get; set; }
    }
}
