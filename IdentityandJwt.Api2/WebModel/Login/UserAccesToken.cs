﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.WebModel.Login
{
    public class UserAccesToken
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string  Password { get; set; }


    }
}
