using BooksMiddletier.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Requests
{
    public class UpdateUserLocationRequest
    {
        public Guid UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
