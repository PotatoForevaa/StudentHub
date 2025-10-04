using Microsoft.AspNetCore.Identity;
using StudentHub.Domain.Entities;

namespace StudentHub.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
