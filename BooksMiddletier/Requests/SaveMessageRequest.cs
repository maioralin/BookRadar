using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class SaveMessageRequest
    {
        public int RequestId { get; set; }
        public Guid From { get; set; }
        public Guid To { get; set; }
        public string Message { get; set; }
    }
}
