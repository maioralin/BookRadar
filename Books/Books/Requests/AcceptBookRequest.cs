using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Requests
{
    public class AcceptBookRequest
    {
        public Guid BookOfferId { get; set; }
        public Guid BookId { get; set; }
        public Guid RequesterId { get; set; }
        public Guid? ProposedReturnBook { get; set; }
    }
}
