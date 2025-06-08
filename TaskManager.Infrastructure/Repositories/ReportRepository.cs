using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Repositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetCompletedTasksByUserAsync(DateTime sinceDate)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Where(t => t.Status == Domain.Enums.TaskStatusEnum.Completed && t.DueDate >= sinceDate)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithCompletedTasksAsync(IEnumerable<Guid> userIds)
        {
            return await _context.Users
                .Include(u => u.Tasks.Where(t => t.Status == Domain.Enums.TaskStatusEnum.Completed))
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }
    }
} 