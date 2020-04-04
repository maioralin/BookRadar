using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class UserResponse : DefaultResponse
    {
        public User User { get; set; }
    }
}
