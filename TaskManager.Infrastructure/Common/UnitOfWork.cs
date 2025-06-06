using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<User> Users { get; private set; }
        public IRepository<Project> Projects { get; private set; }
        public IRepository<TaskItem> Tasks { get; private set; }
        public IRepository<Comment> Comments { get; private set; }
        public IRepository<TaskHistory> TaskHistories { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            Projects = new Repository<Project>(context);
            Tasks = new Repository<TaskItem>(context);
            Comments = new Repository<Comment>(context);
            TaskHistories = new Repository<TaskHistory>(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }

}
