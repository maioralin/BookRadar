using BooksMiddletier.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class BookRequestResponse : DefaultResponse
    {
        public BookRequest BookRequest { get; set; }
    }
}
