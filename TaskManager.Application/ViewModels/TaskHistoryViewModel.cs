namespace TaskManager.Application.ViewModels
{
    public class TaskHistoryViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}