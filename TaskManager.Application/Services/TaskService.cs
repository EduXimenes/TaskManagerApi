using AutoMapper;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskViewModel>> GetAllAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }

        public async Task<TaskViewModel?> GetByIdAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(id);
            return task == null ? null : _mapper.Map<TaskViewModel>(task);
        }

        public async Task<TaskViewModel> CreateAsync(CreateTaskInputModel inputModel)
        {
            var taskCount = await _unitOfWork.Tasks.CountByProjectIdAsync(inputModel.ProjectId);
            if (taskCount >= 20)
            {
                throw new InvalidOperationException("Limite de 20 tarefas por projeto atingido.");
            }
            var task = _mapper.Map<TaskItem>(inputModel);
            await _unitOfWork.Tasks.AddAsync(task);

            var history = new TaskHistory
            {
                TaskItemId = task.Id,
                UserId = task.UserId,
                Description = "Tarefa criada",
                TaskStatusEnum = task.Status,
                ChangeDate = DateTime.UtcNow
            };

            await _unitOfWork.TasksHistories.AddAsync(history);
            await _unitOfWork.CommitAsync();

            var createdTask = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(task.Id);
            return _mapper.Map<TaskViewModel>(createdTask);
        }

        public async Task UpdateAsync(Guid id, UpdateTaskInputModel inputModel)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Tarefa com id {id} não encontrada.");
            }
            var oldStatus = task.Status;
            var changes = new Dictionary<string, (object? oldValue, object? newValue)>();

            if (inputModel.Status != default)
            {
                task.Status = inputModel.Status;
                changes.Add("Status", (oldStatus, inputModel.Status));
            }

            if (!string.IsNullOrEmpty(inputModel.Title))
            {
                changes.Add("Título", (task.Title, inputModel.Title));
                task.Title = inputModel.Title;
            }

            if (!string.IsNullOrEmpty(inputModel.Description))
            {
                changes.Add("Descrição", (task.Description, inputModel.Description));
                task.Description = inputModel.Description;
            }

            if (inputModel.DueDate != default)
            {
                changes.Add("Data de entrega", (task.DueDate, inputModel.DueDate));
                task.DueDate = inputModel.DueDate;
            }

            if (inputModel.UserId != default)
            {
                changes.Add("Usuário responsável", (task.UserId, inputModel.UserId));
                task.UserId = inputModel.UserId;
            }

            if (oldStatus != TaskStatusEnum.Completed && task.Status == TaskStatusEnum.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
                changes.Add("Conclusão", (null, "Tarefa marcada como concluída"));
            }
            else if (oldStatus == TaskStatusEnum.Completed && task.Status != TaskStatusEnum.Completed)
            {
                task.CompletedAt = null;
                changes.Add("Conclusão", (null, "Tarefa desmarcada como concluída"));
            }

            await _unitOfWork.Tasks.UpdateAsync(task);

            foreach (var change in changes)
            {
                var history = new TaskHistory
                {
                    TaskItemId = task.Id,
                    UserId = task.UserId,
                    Description = FormatChangeDescription(change.Key, change.Value.oldValue, change.Value.newValue),
                    TaskStatusEnum = task.Status,
                    ChangeDate = DateTime.UtcNow
                };

                await _unitOfWork.TasksHistories.AddAsync(history);
            }

            await _unitOfWork.CommitAsync();
        }

        private string FormatChangeDescription(string propertyName, object? oldValue, object? newValue)
        {
            if (oldValue == null)
            {
                return $"{propertyName}: {newValue}";
            }
            if (oldValue is DateTime oldDate && newValue is DateTime newDate)
            {
                return $"{propertyName} alterado de {oldDate:dd/MM/yyyy} para {newDate:dd/MM/yyyy}";
            }
            if (oldValue is TaskStatusEnum oldStatus && newValue is TaskStatusEnum newStatus)
            {
                return $"{propertyName} alterado de {oldStatus} para {newStatus}";
            }
            return $"{propertyName} alterado de '{oldValue}' para '{newValue}'";
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Tarefa com id {id} não encontrada.");
            }
            await _unitOfWork.Tasks.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<TaskViewModel>> GetByProjectIdAsync(Guid projectId)
        {
            var tasks = await _unitOfWork.Tasks.GetByProjectIdAsync(projectId);
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }

        public async Task<IEnumerable<TaskViewModel>> GetByUserIdAsync(Guid userId)
        {
            var tasks = await _unitOfWork.Tasks.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }
    }
}
