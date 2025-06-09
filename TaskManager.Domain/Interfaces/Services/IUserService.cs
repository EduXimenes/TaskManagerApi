using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();
        Task<UserViewModel?> GetByIdAsync(Guid id);
        Task<UserViewModel> CreateAsync(CreateUserInputModel inputModel);
        Task UpdateAsync(Guid id, CreateUserInputModel inputModel);
        Task DeleteAsync(Guid id);
        Task<bool> IsManagerAsync(Guid userId);
    }
}