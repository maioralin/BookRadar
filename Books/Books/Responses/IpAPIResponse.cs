using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Responses
{
    public class IpAPIResponse
    {
        public string Status { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
