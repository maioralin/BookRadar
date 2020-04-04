using Books.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Requests
{
    public class UpdateUserLocationRequest
    {
        public Guid UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
