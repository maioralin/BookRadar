using Books.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Requests
{
    public class AddBookRequest
    {
        public Book Book { get; set; }
        public Guid UserId { get; set; }
        public bool Exchange { get; set; }
        public bool OwnBook { get; set; }
        public bool Giveaway { get; set; }
        public string Description { get; set; }
    }
}
