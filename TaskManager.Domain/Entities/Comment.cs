using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }

}
