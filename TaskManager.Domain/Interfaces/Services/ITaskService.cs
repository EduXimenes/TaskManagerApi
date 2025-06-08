using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Domain.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskViewModel>> GetAllAsync();
        Task<TaskViewModel?> GetByIdAsync(Guid id);
        Task<TaskViewModel> CreateAsync(CreateTaskInputModel inputModel);
        Task UpdateAsync(Guid id, UpdateTaskInputModel inputModel);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TaskViewModel>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<TaskViewModel>> GetByUserIdAsync(Guid userId);
    }
}