using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Domain.Interfaces.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectViewModel>> GetAllAsync();
        Task<ProjectViewModel?> GetByIdAsync(Guid id);
        Task<ProjectViewModel> CreateAsync(CreateProjectInputModel inputModel);
        Task UpdateAsync(Guid id, CreateProjectInputModel inputModel);
        Task DeleteAsync(Guid id);
    }
}