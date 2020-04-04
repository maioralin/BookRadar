using BooksMiddletier.ISBNDB;
using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class SearchByISBNResponse: DefaultResponse
    {
        public BookMinInfoByISBN Book { get; set; }
        public bool Found { get; set; }
    }
}
