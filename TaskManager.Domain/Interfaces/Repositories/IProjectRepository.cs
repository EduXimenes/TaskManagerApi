using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetByIdWithUserAndTasksAsync(Guid id);
        Task<IEnumerable<Project>> GetAllWithUserAsync();
        Task<Project?> GetByIdWithTasksAsync(Guid id);
    }
}