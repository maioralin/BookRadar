using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.OtherClasses
{
    public class BookRequest
    {
        public int Id { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public BookOffer BookOffer { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public bool IsUserReviewed { get; set; }
        public bool Donate { get; set; }
        public Guid BookId { get; set; }
        public Guid? ProposedBookId { get; set; }
        public bool AcceptedExchange { get; set; }
        public bool PermanentExchange { get; set; }
        public Book ProposedBook { get; set; }
        public bool Giveaway { get; set; }
        public string Cover { get; set; }
    }
}
