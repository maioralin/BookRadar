using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
