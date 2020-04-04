using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class BorrowHistoryResponse: DefaultResponse
    {
        public List<BorrowHistorySQL> BorrowedBooks { get; set; }
    }
}
