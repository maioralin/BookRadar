using System;
using System.Collections.Generic;
using System.Text;

namespace Books.SqlClasses
{
    public class SqlBook
    {
        public int ISBN { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public Guid BookId { get; set; }
        public string Cover { get; set; }
    }
}
