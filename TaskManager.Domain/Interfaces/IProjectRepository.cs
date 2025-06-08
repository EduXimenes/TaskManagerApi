using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithUserAndTasksAsync(Guid id);
    Task<IEnumerable<Project>> GetAllWithUserAsync();
    Task<Project?> GetByIdWithTasksAsync(Guid id);
}
