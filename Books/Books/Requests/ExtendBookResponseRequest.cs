using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class ExtendBookResponseRequest
    {
        public Guid OfferId { get; set; }
        public bool Answer { get; set; }
        public DateTime ProposedDate { get; set; }
    }
}
