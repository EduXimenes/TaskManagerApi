using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Repositories;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Domain.Interfaces.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentViewModel>> GetByTaskIdAsync(Guid taskId);
        Task<CommentViewModel?> GetByIdAsync(Guid id);
        Task<CommentViewModel> CreateAsync(CreateCommentInputModel inputModel);
        Task UpdateAsync(Guid id, CreateCommentInputModel inputModel);
        Task DeleteAsync(Guid id);
    }
}
