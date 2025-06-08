namespace TaskManager.Application.ViewModels
{
    public class PerformanceReportViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public double AverageCompletedTasksLast30Days { get; set; }
    }
}
