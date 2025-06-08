using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Repositories;

namespace TaskManager.Domain.Interfaces.Common
{
    public interface IUnitOfWork
    {
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }
        IRepository<User> Users { get; }
        ICommentRepository Comments { get; }
        IReportRepository Reports { get; }
        IRepository<TaskHistory> TasksHistories { get; }
        Task<int> CommitAsync();
    }
}
