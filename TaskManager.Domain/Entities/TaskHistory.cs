using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskHistory : BaseEntity
    {
        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public TaskStatusEnum? TaskStatusEnum { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}
