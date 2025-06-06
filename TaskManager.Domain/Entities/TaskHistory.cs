using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities
{
    public class TaskHistory : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }

}
