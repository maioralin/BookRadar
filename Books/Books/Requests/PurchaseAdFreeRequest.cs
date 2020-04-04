using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class PurchaseAdFreeRequest
    {
        public Guid UserId { get; set; }
        public string PurchaseId { get; set; }
        public string PurchaseToken { get; set; }
    }
}
