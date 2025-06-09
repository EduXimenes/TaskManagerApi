using AutoMapper;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.Enums;

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

        public async Task<ProjectViewModel> CreateAsync(CreateProjectInputModel inputModel)
        {
            var project = _mapper.Map<Project>(inputModel);
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CommitAsync();

            var createdProject = await _unitOfWork.Projects.GetByIdWithUserAndTasksAsync(project.Id);
            return _mapper.Map<ProjectViewModel>(createdProject);
        }

        public async Task UpdateAsync(Guid id, CreateProjectInputModel inputModel)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null)
                throw new KeyNotFoundException($"Projeto com id {id} não encontrado.");

            _mapper.Map(inputModel, project);
            await _unitOfWork.Projects.UpdateAsync(project);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null)
                throw new KeyNotFoundException($"Projeto com id {id} não encontrado.");

            var hasIncompleteTasks = await _unitOfWork.Tasks.AnyAsync(
                t => t.ProjectId == id && t.Status != TaskStatusEnum.Completed);

            if (hasIncompleteTasks)
                throw new InvalidOperationException("Não é possível excluir o projeto com tarefas pendentes ou em andamento.");

            await _unitOfWork.Projects.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
