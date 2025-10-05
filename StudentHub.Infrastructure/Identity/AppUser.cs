﻿using Microsoft.AspNetCore.Identity;

namespace StudentHub.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
    }
}
