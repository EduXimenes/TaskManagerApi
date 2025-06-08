using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IProjectRepository Projects { get; }
        IRepository<TaskItem> Tasks { get; }
        IRepository<Comment> Comments { get; }
        IRepository<TaskHistory> TaskHistories { get; }
        Task<int> CommitAsync();
    }
}
