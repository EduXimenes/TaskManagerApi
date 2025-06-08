using TaskManager.Domain.Entities;

namespace TaskManager.Domain.ViewModels
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int TasksCount { get; set; }
        public ICollection<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }
}
