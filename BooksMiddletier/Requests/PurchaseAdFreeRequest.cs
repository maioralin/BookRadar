using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class PurchaseAdFreeRequest
    {
        public Guid UserId { get; set; }
        public string PurchaseId { get; set; }
        public string PurchaseToken { get; set; }
    }
}
