using Domain.Enum;

namespace ManagementSystem.Models
{
    public class UpdateTaskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public Periority? Priority { get; set; }

        public DateTime DueDate { get; set; }

        public string? AssignedUserId { get; set; }

        public List<SelectListItem>? Users { get; set; }
    }
}
