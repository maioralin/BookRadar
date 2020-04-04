using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class GetMessagesResponse : DefaultResponse
    {
        public List<MessageSQL> Messages { get; set; }
    }
}
