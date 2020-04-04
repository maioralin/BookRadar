using System;
using System.Collections.Generic;
using System.Text;

namespace Books.OtherClasses
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Id { get; set; }
        public string Last_Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public Picture Picture { get; set; }
        public string Email { get; set; }
    }

    public class PictureData
    {
        public string Url { get; set; }
    }

    public class Picture
    {
        public PictureData Data { get; set; }
    }
}
