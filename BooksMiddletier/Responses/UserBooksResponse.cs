﻿using BooksMiddletier.OtherClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class UserBooksResponse: DefaultResponse
    {
        public List<Book> Books { get; set; }
        public int TotalRows { get; set; }
    }
}
