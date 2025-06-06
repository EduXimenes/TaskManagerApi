using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Default;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    }
}
