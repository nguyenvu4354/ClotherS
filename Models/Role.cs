﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ClotherS.Models
{
    public class Role : IdentityRole<int>
    {
        public bool Disable { get; set; } = false;
    }
}
