using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class UserReview
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int BookQuality { get; set; }
        public int TimeQuality { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public int TotalRows { get; set; }
    }
}
