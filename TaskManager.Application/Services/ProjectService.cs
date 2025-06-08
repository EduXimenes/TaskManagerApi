using AutoMapper;
using TaskManager.Application.InputModels;
using TaskManager.Application.Interfaces;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectViewModel>> GetAllAsync()
        {
            var projects = await _unitOfWork.Projects.GetAllWithUserAsync();
            return _mapper.Map<IEnumerable<ProjectViewModel>>(projects);
        }

        public async Task<ProjectViewModel?> GetByIdAsync(Guid id)
        {
            var project = await _unitOfWork.Projects.GetByIdWithUserAndTasksAsync(id);
            return project == null ? null : _mapper.Map<ProjectViewModel>(project);
        }

        public async Task<Guid> CreateAsync(CreateProjectInputModel inputModel)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(inputModel.UserId)
                ?? throw new Exception("Usuário não encontrado");

            var project = _mapper.Map<Project>(inputModel);
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CommitAsync();

            return project.Id;
        }

        public async Task UpdateAsync(Guid id, CreateProjectInputModel inputModel)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id)
                ?? throw new Exception("Projeto não encontrado");

            project.Name = inputModel.Name;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _unitOfWork.Projects.GetByIdWithTaskAsync(id)
                ?? throw new Exception("Projeto não encontrado");

            if (project.Tasks.Any(t => t.Status != Domain.Enums.TaskStatus.Completed))
                throw new Exception("Não é possível excluir um projeto com tarefas pendentes.");

            await _unitOfWork.Projects.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
