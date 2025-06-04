using Domain.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Models
{
    public class UpdateTaskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority? PriorityType { get; set; }

        public DateTime DueDate { get; set; }

        public string? AssignedUserId { get; set; }

        public List<SelectListItem>? Users { get; set; }
    }
}
