using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class User
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public int BookCount { get; set; }
        public decimal BookQuality { get; set; }
        public decimal TimeQuality { get; set; }
    }
}
