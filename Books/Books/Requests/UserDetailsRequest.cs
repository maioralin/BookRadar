using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class UserDetailsRequest
    {
        public Guid RequesterId { get; set; }
        public Guid UserId { get; set; }
    }
}
