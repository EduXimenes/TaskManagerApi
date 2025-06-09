using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<TaskItem>> GetCompletedTasksByUserAsync(DateTime sinceDate);
        Task<IEnumerable<(User user, int completedTasks)>> GetUserPerformanceAsync(DateTime sinceDate);

    }
}