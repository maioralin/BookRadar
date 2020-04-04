using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.SqlClasses
{
    public class UserDetails
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string MyReferralCode { get; set; }
        public string InviteCode { get; set; }
        public string PurchaseId { get; set; }
    }
}
