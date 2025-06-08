using TaskManager.Domain.Enums;
using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

namespace TaskManager.Domain.InputModels
{
    public class UpdateTaskInputModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatusEnum Status { get; set; }
        public Guid UserId { get; set; }
    }
}
