using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
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
                .Where(t => t.Status == TaskStatusEnum.Completed && t.DueDate >= sinceDate)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<(User user, int completedTasks)>> GetUserPerformanceAsync(DateTime sinceDate)
        {
            var userTaskGroups = await _context.Tasks
                .Where(t => t.Status == TaskStatusEnum.Completed && t.DueDate >= sinceDate)
                .GroupBy(t => t.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    CompletedCount = g.Count()
                })
                .ToListAsync();

            var userIds = userTaskGroups.Select(g => g.UserId).ToList();

            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var result = users.Select(user =>
            {
                var completed = userTaskGroups.First(g => g.UserId == user.Id).CompletedCount;
                return (user, completed);
            });

            return result;
        }
    }
} 