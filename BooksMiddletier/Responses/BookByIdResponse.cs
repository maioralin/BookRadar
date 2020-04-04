using BooksMiddletier.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class BookByIdResponse: DefaultResponse
    {
        public Book Book { get; set; }
    }
}
