using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class FacebookLoginRequest
    {
        public string Email { get; set; }
        public string ID { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
    }
}
