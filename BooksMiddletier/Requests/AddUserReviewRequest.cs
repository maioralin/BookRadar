using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class AddUserReviewRequest
    {
        public Guid UserId { get; set; }
        public int RequestId { get; set; }
        public int BookAspect { get; set; }
        public int ReturnTime { get; set; }
        public string Comment { get; set; }
        public Guid ReviewerId { get; set; }
    }
}
