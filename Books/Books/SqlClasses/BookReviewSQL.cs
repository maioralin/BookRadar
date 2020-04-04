using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.SqlClasses
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
