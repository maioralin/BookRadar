using BooksMiddletier.OtherClasses;
using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class UserDetailsResponse: DefaultResponse
    {
        public UserDetails Info { get; set; }
    }
}
