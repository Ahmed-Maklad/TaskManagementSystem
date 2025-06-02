using Domain.Enum;

namespace Application.DTOs
{
    public class GetTasksDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Periority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string? AssignedUserId { get; set; }

        public string? AssignedFullName { get; set; }
    }

}
