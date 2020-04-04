using System;
using System.Collections.Generic;
using System.Text;

namespace Books.SqlClasses
{
    public class RequestMinInfo
    {
        public int Id { get; set;}
        public string Title { get; set; }
        public string Authors { get; set; }
        public string SmallCover { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public int TotalRows { get; set; }
    }
}
