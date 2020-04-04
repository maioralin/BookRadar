using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class UserResponse: DefaultResponse
    {
        public User User { get; set; }
    }
}
