namespace TaskManager.Application.ViewModels
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int TasksCount { get; set; }
    }
}
