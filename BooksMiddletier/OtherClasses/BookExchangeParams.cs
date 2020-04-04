using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.OtherClasses
{
    public class BookExchangeParams
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public Guid ToUserId { get; set; }
    }
}
