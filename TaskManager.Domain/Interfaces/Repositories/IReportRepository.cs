using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<TaskItem>> GetCompletedTasksByUserAsync(DateTime sinceDate);
        Task<IEnumerable<User>> GetUsersWithCompletedTasksAsync(IEnumerable<Guid> userIds);
    }
}