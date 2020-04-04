using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.SqlClasses
{
    public class MessageSQL
    {
        public Guid MessageFrom { get; set; }
        public Guid MessageTo { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentDate { get; set; }
    }
}
