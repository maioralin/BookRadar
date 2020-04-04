using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.OtherClasses
{
    public class ListBook
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
    }
}
