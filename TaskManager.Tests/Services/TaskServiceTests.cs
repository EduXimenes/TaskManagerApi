using Moq;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using Xunit;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateTaskAsync_ShouldAddTask_WhenProjectIsValid()
    {
        var project = new Project();
        var projectId = project.Id;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var mockTaskRepo = new Mock<IRepository<TaskItem>>();
        mockUnitOfWork.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

        var service = new TaskService(mockUnitOfWork.Object);

        var result = await service.CreateTaskAsync(projectId, "Title", "Desc", DateTime.UtcNow, TaskPriority.Medium);

        mockTaskRepo.Verify(t => t.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldThrow_WhenProjectHas20Tasks()
    {
        var project = new Project { Tasks = new List<TaskItem>(new TaskItem[20]) };
        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Projects.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(project);

        var service = new TaskService(mockUow.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.CreateTaskAsync(Guid.NewGuid(), "T", "D", DateTime.UtcNow, TaskPriority.High));
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldTrackChangesAndCommit()
    {
        var task = new TaskItem(TaskPriority.Medium)
        {
            Title = "Old",
            Description = "Old desc",
            DueDate = DateTime.UtcNow,
            Status = TaskStatus.Pending
        };

        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Tasks.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var mockHistoryRepo = new Mock<IRepository<TaskHistory>>();
        mockUow.Setup(u => u.TaskHistories).Returns(mockHistoryRepo.Object);

        var service = new TaskService(mockUow.Object);

        await service.UpdateTaskAsync(task.Id, "New", "New desc", task.DueDate.AddDays(1), TaskStatus.Completed, Guid.NewGuid());

        mockHistoryRepo.Verify(h => h.AddAsync(It.IsAny<TaskHistory>()), Times.Once);
        mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldLogHistoryAndDeleteTask()
    {
        var task = new TaskItem(TaskPriority.Low)
        {
            Title = "Task"
        };

        var mockUow = new Mock<IUnitOfWork>();

        var mockHistoryRepo = new Mock<IRepository<TaskHistory>>();
        var mockTaskRepo = new Mock<IRepository<TaskItem>>();

        mockUow.Setup(u => u.TaskHistories).Returns(mockHistoryRepo.Object);
        mockUow.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

        mockUow.Setup(u => u.Tasks.GetByIdAsync(task.Id)).ReturnsAsync(task);
        var service = new TaskService(mockUow.Object);

        await service.DeleteTaskAsync(task.Id, Guid.NewGuid());

        mockHistoryRepo.Verify(h => h.AddAsync(It.IsAny<TaskHistory>()), Times.Once);
        mockTaskRepo.Verify(t => t.DeleteAsync(task.Id), Times.Once);
        mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }
}
