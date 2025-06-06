using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}