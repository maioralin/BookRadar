using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class BookDetailsSQL
    {
        public int ISBN { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string MediumCover { get; set; }
        public Guid? BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public bool Donate { get; set; }
        public bool Giveaway { get; set; }
        public string Description { get; set; }
    }
}
