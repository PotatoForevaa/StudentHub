using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppIdentityDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    }
}
