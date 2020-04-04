using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class SaveMessageRequest
    {
        public int RequestId { get; set; }
        public Guid From { get; set; }
        public Guid To { get; set; }
        public string Message { get; set; }
    }
}
