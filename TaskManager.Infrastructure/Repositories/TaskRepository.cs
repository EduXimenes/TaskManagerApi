using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Repositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .Include(t => t.TaskHistories)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> CountByProjectIdAsync(Guid projectId)
        {
            return await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
        }
        public async Task<bool> AnyAsync(Expression<Func<TaskItem, bool>> taskStatus)
        {
            return await _context.Tasks.AnyAsync(taskStatus);
        }
    }
} 