using System;
using System.Collections.Generic;
using System.Text;

namespace Books.OtherClasses
{
    public class BookOffer
    {
        public Guid? Id { get; set; }
        public int BookRequestId { get; set; }
        public DateTime? ProposedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public bool BookAccepted { get; set; }
        public bool ReturnOffered { get; set; }
        public DateTime? ExtendedDate { get; set; }
    }
}
