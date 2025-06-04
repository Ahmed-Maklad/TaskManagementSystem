using Domain.Enum;

namespace Domain.Entities
{
    public class UserTask : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority? PriorityType { get; set; }
        public DateTime DueDate { get; set; }
        public int AssignedUserId { get; set; }

        public ApplicationUser AssignedUser { get; set; }
    }
}
