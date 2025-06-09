using TaskManager.Domain.Enums;

namespace TaskManager.Domain.ViewModels
{
    public class TaskHistoryViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public TaskStatusEnum? TaskStatusEnum { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ModifiedByUserName { get; set; } = string.Empty;
    }
}