using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class BookReviewSQL
    {
        public int Id { get; set; }
        public int ISBN { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
    }
}
