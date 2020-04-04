using System;
using System.Collections.Generic;
using System.Text;

namespace Books.OtherClasses
{
    public class Book
    {
        public int ISBN { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string SmallCover { get; set; }
        public string MediumCover { get; set; }
        public string LargeCover { get; set; }
        public OwnerDetails Owner { get; set; }
        public Guid Id { get; set; }
        public double Distance { get; set; }
        public Guid? BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public string GoodreadsId { get; set; }
        public string WorkId { get; set; }
        public string AuthorName { get; set; }
        public bool Donate { get; set; }
        public bool Giveaway { get; set; }
        public string Description { get; set; }
    }
}
