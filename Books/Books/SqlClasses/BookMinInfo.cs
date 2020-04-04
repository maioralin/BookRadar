using System;
using System.Collections.Generic;
using System.Text;

namespace Books.SqlClasses
{
    public class BookMinInfo
    {
        public int ISBN { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public double Distance { get; set; }
        public Guid BookId { get; set; }
        public string Cover { get; set; }
        public int TotalRows { get; set; }
        public Guid OwnerId { get; set; }
    }
}
