using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateTaskAsync(Guid projectId, string title, string description, DateTime dueDate, TaskPriority priority)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
                ?? throw new Exception("Projeto não encontrado");

            if (project.Tasks.Count >= 20)
                throw new Exception("Limite de tarefas excedido para este projeto.");

            var task = new TaskItem(priority)
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                ProjectId = projectId
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.CommitAsync();

            return task.Id;
        }
        public async Task UpdateTaskAsync(
               Guid taskId,
               string? newTitle,
               string? newDescription,
               DateTime? newDueDate,
               TaskStatus? newStatus,
               Guid userId)
            {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId)
                ?? throw new Exception("Tarefa não encontrada");

            var changes = new List<string>();

            if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != task.Title)
            {
                task.Title = newTitle;
                changes.Add("Título atualizado");
            }

            if (!string.IsNullOrWhiteSpace(newDescription) && newDescription != task.Description)
            {
                task.Description = newDescription;
                changes.Add("Descrição atualizada");
            }

            if (newDueDate.HasValue && newDueDate.Value != task.DueDate)
            {
                task.DueDate = newDueDate.Value;
                changes.Add("Data de vencimento alterada");
            }

            if (newStatus.HasValue && newStatus.Value != task.Status)
            {
                task.Status = newStatus.Value;
                changes.Add("Status alterado");
            }

            if (changes.Any())
            {
                await _unitOfWork.TaskHistories.AddAsync(new TaskHistory
                {
                    TaskItemId = taskId,
                    ChangeDate = DateTime.UtcNow,
                    UserId = userId,
                    Description = string.Join(". ", changes) + "."
                });
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId)
                ?? throw new Exception("Tarefa não encontrada");

            await _unitOfWork.TaskHistories.AddAsync(new TaskHistory
            {
                TaskItemId = task.Id,
                ChangeDate = DateTime.UtcNow,
                UserId = userId,
                Description = $"Tarefa '{task.Title}' foi removida."
            });

            await _unitOfWork.Tasks.DeleteAsync(task.Id);
            await _unitOfWork.CommitAsync();
        }

    }
}
