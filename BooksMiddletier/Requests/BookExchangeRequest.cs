using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class BookExchangeRequest
    {
        public int RequestId { get; set; }
        public Guid OfferedBookId { get; set; }
        public bool PermanentExchange { get; set; }
        public bool AcceptedExchange { get; set; }
    }
}
