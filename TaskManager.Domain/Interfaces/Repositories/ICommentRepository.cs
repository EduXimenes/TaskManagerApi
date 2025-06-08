using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;

namespace TaskManager.Domain.Interfaces.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByTaskIdAsync(Guid taskId);
        Task<Comment?> GetByIdWithDetailsAsync(Guid id);
    }
} 