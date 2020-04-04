﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Books.SqlClasses
{
    public class BookRequestSQL
    {
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public Guid? OfferId { get; set; }
        public DateTime? ProposedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public bool BookAccepted { get; set; }
        public bool ReturnOffered { get; set; }
        public bool Donate { get; set; }
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Cover { get; set; }
    }
}
