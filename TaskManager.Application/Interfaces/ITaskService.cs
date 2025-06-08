using TaskManager.Domain.Enums;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain.Interfaces
{
    public interface ITaskService
    {
        Task<Guid> CreateTaskAsync(Guid projectId, string title, string description, DateTime dueDate, TaskPriority priority);

        Task UpdateTaskAsync(Guid taskId, string? newTitle, string? newDescription, DateTime? newDueDate, TaskStatus? newStatus, Guid userId);
     
        Task DeleteTaskAsync(Guid taskId, Guid userId);

    }
}
