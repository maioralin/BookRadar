using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class BookMinInfoByISBN
    {
        public string ISBN13 { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Authors { get; set; }
    }
}
