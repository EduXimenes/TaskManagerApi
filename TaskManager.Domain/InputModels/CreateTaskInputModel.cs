using TaskManager.Domain.Enums;

namespace TaskManager.Domain.InputModels
{
    public class CreateTaskInputModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; } 
    }
}
