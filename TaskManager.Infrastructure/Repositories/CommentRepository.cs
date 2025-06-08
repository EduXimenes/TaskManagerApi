using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Repositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.TaskItemId == taskId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.TaskItem)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
} 