using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books
{

    public class MasterPageMenuItem
    {
        public MasterPageMenuItem()
        {
            TargetType = typeof(MasterPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}