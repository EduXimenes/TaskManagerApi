using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
        public TaskPriority Priority { get; private set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = default!;
        public Guid UserId { get; set; }
        public User AssignedUser { get; set; } = default!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();

        public TaskItem(TaskPriority priority)
        {
            Priority = priority;
        }
    }
}
