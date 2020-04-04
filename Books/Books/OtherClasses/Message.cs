using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Books.OtherClasses
{
    public class Message
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime SentDate { get; set; }
        public Color FrameColor { get; set; }
        public Thickness Margin { get; set; }
    }
}
