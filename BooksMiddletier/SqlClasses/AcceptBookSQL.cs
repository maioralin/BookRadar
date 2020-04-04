using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class AcceptBookSQL
    {
        public bool Donate { get; set; }
        public Guid RequesterId { get; set; }
        public int RequestId { get; set; }
        public Guid BookId { get; set; }
        public Guid? ProposedBookId { get; set; }
        public bool? PermanentExchange { get; set; }
        public Guid OwnerId { get; set; }
        public bool Giveaway { get; set; }
    }
}
