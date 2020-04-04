using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class UserReviewsResponse : DefaultResponse
    {
        public List<UserReview> Reviews { get; set; }
    }
}
