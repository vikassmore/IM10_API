﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class AuthModel
    {
        public long UserId { get; set; }

       // public string Username { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public int RoleId { get; set; }
        public string Role { get; set; } =null!;
        public string Token { get; set; } = null!;
       public string FullName { get;set; } = null!;

        public string MobileNo { get; set; } = null!;


    }
}
