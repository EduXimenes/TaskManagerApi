using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Repositories;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        private IProjectRepository _projects;
        private ITaskRepository _tasks;
        private IRepository<User> _users;
        private ICommentRepository _comments;
        private IReportRepository _reports;
        private IRepository<TaskHistory> _tasksHistories;


        private bool _disposed;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);
        public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
        public IRepository<User> Users => _users ??= new Repository<User>(_context);
        public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
        public IReportRepository Reports => _reports ??= new ReportRepository(_context);
        public IRepository<TaskHistory> TasksHistories => _tasksHistories ??= new Repository<TaskHistory>(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}
