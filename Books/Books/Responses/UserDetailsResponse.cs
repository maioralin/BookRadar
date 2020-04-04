using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class UserDetailsResponse: DefaultResponse
    {
        public UserDetails Info { get; set; }
    }
}
