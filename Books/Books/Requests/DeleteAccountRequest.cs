﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Requests
{
    public class DeleteAccountRequest
    {
        public Guid UserId { get; set; }
    }
}
