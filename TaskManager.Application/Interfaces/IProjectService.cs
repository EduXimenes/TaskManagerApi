namespace TaskManager.Domain.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectViewModel>> GetAllAsync();
        Task<ProjectViewModel> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateProjectInputModel inputModel);
        Task UpdateAsync(Guid id, CreateProjectInputModel inputModel);
        Task DeleteAsync(Guid id);
    }
}
