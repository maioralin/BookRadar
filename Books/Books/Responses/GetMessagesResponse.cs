using Books.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class GetMessagesResponse : DefaultResponse
    {
        public List<MessageSQL> Messages { get; set; }
    }
}
