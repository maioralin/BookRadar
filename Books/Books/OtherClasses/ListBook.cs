using System;
using System.Collections.Generic;
using System.Text;

namespace Books.OtherClasses
{
    public class ListBook
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
    }
}
