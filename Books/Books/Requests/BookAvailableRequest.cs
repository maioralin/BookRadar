using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class BookAvailableRequest
    {
        public Guid BookId { get; set; }
        public bool Available { get; set; }
    }
}
