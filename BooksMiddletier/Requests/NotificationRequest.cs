using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
