//using Moq;
//using TaskManager.Application.Services;
//using TaskManager.Domain.Entities;
//using TaskManager.Domain.Enums;
//using TaskManager.Domain.Interfaces;
//using Xunit;
//using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

//public class ReportServiceTests
//{
//    [Fact]
//    public async Task GenerateAverageTasksCompletedReportAsync_ShouldReturnCorrectValue()
//    {
//        // Arrange
//        var completedTasks = new List<TaskItem>
//        {
//            new TaskItem(TaskPriority.Low) { Status = TaskStatusEnum.Completed, DueDate = DateTime.UtcNow.AddDays(-5), UserId = Guid.NewGuid() },
//            new TaskItem(TaskPriority.Medium) { Status = TaskStatusEnum.Completed, DueDate = DateTime.UtcNow.AddDays(-10), UserId = Guid.NewGuid() },
//            new TaskItem(TaskPriority.High) { Status = TaskStatusEnum.Completed, DueDate = DateTime.UtcNow.AddDays(-15), UserId = Guid.NewGuid() },
//        };

//        var mockTaskRepo = new Mock<IRepository<TaskItem>>();
//        var mockUow = new Mock<IUnitOfWork>();

//        mockTaskRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(completedTasks);
//        mockUow.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

//        var service = new ReportService(mockUow.Object);

//        // Act
//        var result = await service.GenerateAverageTasksCompletedReportAsync();

//        // Assert
//        Assert.Equal(3, result.TotalCompletedTasks);
//    }
//}
