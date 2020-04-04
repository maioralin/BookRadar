using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class ExtendBookRequest
    {
        public Guid OfferId { get; set; }
        public DateTime ProposedDate { get; set; }
    }
}
