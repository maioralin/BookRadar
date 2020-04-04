using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class SearchByISBNResponse : DefaultResponse
    {
        public BookMinInfoByISBN Book { get; set; }
        public bool Found { get; set; }
    }
}
