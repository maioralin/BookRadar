using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class ReminderSQL
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
    }
}
