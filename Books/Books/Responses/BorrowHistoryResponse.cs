using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class BorrowHistoryResponse : DefaultResponse
    {
        public List<BorrowHistorySQL> BorrowedBooks { get; set; }
    }
}
