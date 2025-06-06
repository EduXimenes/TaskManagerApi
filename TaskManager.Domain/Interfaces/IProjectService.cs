namespace TaskManager.Domain.Interfaces
{
    public interface IProjectService
    {
        Task<Guid> CreateProjectAsync(Guid userId, string name);
    }
}
