using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class BookExchangeRequest
    {
        public int RequestId { get; set; }
        public Guid OfferedBookId { get; set; }
        public bool PermanentExchange { get; set; }
        public bool AcceptedExchange { get; set; }
    }
}
