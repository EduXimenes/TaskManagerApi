using TaskManager.Domain.Enums;
using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

namespace TaskManager.Domain.ViewModels
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatusEnum Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ICollection<TaskHistoryViewModel> History { get; set; } = new List<TaskHistoryViewModel>();
    }
}
