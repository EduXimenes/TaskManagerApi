using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<TaskItem?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync();
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId);
        Task<int> CountByProjectIdAsync(Guid projectId);
    }
}