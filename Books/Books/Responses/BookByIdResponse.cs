using Books.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class BookByIdResponse: DefaultResponse
    {
        public Book Book { get; set; }
    }
}
