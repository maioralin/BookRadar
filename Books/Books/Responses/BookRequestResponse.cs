using Books.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class BookRequestResponse : DefaultResponse
    {
        public BookRequest BookRequest { get; set; }
    }
}
