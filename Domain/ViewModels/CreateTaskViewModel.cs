using Domain.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Models
{
    public class CreateTaskViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority PriorityType { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsAdmin { get; set; }

        public string? AssignedUserId { get; set; }

        public List<SelectListItem>? Users { get; set; }
    }
    //public class SelectListItem
    //{
    //    public string? Value { get; set; }
    //    public string? Text { get; set; }
    //}
}
