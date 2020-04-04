using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.OtherClasses
{
    public class BookExchangeParams
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public Guid ToUserId { get; set; }
    }
}
