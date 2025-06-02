using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public virtual ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();
    }
}
