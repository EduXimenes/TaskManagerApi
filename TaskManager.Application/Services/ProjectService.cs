using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateProjectAsync(Guid userId, string name)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new Exception("Usuário não encontrado");

            var project = new Project { Name = name, UserId = userId };
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CommitAsync();

            return project.Id;
        }
    }
}
