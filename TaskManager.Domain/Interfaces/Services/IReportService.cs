using TaskManager.Domain.ViewModels;

namespace TaskManager.Domain.Interfaces.Services
{
    public interface IReportService
    {
        
        Task<IEnumerable<TaskViewModel>> GetCompletedTasksByUserAsync(DateTime sinceDate);
        Task<IEnumerable<PerformanceReportViewModel>> GetPerformanceReportAsync();
    }
} 