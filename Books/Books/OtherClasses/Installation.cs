using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.OtherClasses
{
    public class Installation
    {
        public string installationId { get; set; }
        public string[] tags { get; set; }
        public string platform { get; set; }
        public string pushChannel { get; set; }
    }
}
