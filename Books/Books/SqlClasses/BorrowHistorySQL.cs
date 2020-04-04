using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.SqlClasses
{
    public class BorrowHistorySQL
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int TotalRows { get; set; }
        public string Cover { get; set; }
    }
}
