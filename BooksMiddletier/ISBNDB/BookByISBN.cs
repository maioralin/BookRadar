using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.ISBNDB
{
    public class BookByISBN
    {
        public Book book { get; set; }

    }

    public class Book
    {
        public string Format { get; set; }
        public string Image { get; set; }
        public string Title_long { get; set; }
        public string Date_published { get; set; }
        public string[] Subjects { get; set; }
        public string[] Authors { get; set; }
        public string Title { get; set; }
        public string Isbn13 { get; set; }
        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Dimensions { get; set; }
        public string Dewey_decimal { get; set; }
        public string Edition { get; set; }
    }
}
