using Moq;
using AutoMapper;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.ViewModels;
using Xunit;

namespace TaskManager.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new ReportService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCompletedTasksByUserAsync_ShouldReturnCompletedTasks()
        {
            // Arrange
            var sinceDate = DateTime.UtcNow.AddDays(-30);
            var completedTasks = new List<TaskItem>
            {
                new TaskItem(TaskPriority.Medium) 
                { 
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-5)
                },
                new TaskItem(TaskPriority.High)
                {
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            var taskViewModels = new List<TaskViewModel>
            {
                new TaskViewModel { Id = completedTasks[0].Id },
                new TaskViewModel { Id = completedTasks[1].Id }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetAllWithDetailsAsync())
                .ReturnsAsync(completedTasks);
            _mockMapper.Setup(m => m.Map<IEnumerable<TaskViewModel>>(It.IsAny<IEnumerable<TaskItem>>()))
                .Returns(taskViewModels);

            // Act
            var result = await _service.GetCompletedTasksByUserAsync(sinceDate);

            // Assert
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(u => u.Tasks.GetAllWithDetailsAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<TaskViewModel>>(It.IsAny<IEnumerable<TaskItem>>()), Times.Once);
        }

        [Fact]
        public async Task GetPerformanceReportAsync_ShouldReturnUserPerformance()
        {
            // Arrange
            var user1 = new User { Name = "User 1" };
            var user2 = new User { Name = "User 2" };

            var tasks = new List<TaskItem>
            {
                new TaskItem(TaskPriority.Medium)
                {
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-5),
                    AssignedUser = user1
                },
                new TaskItem(TaskPriority.High)
                {
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-10),
                    AssignedUser = user1
                },
                new TaskItem(TaskPriority.Low)
                {
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-15),
                    AssignedUser = user2
                }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetAllWithDetailsAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _service.GetPerformanceReportAsync();

            // Assert
            Assert.Equal(2, result.Count());
            var user1Report = result.First(r => r.UserId == user1.Id);
            var user2Report = result.First(r => r.UserId == user2.Id);

            Assert.Equal(2, user1Report.TotalCompletedTasks);
            Assert.Equal(1, user2Report.TotalCompletedTasks);
            Assert.Equal(2.0/30, user1Report.AverageCompletedTasksLast30Days, 2);
            Assert.Equal(1.0/30, user2Report.AverageCompletedTasksLast30Days, 2);
        }

        [Fact]
        public async Task GetPerformanceReportAsync_ShouldNotIncludeIncompleteTasks()
        {
            // Arrange
            var user = new User { Name = "User 1" };
            var tasks = new List<TaskItem>
            {
                new TaskItem(TaskPriority.Medium)
                {
                    Status = TaskStatusEnum.Completed,
                    CompletedAt = DateTime.UtcNow.AddDays(-5),
                    AssignedUser = user
                },
                new TaskItem(TaskPriority.High)
                {
                    Status = TaskStatusEnum.Pending,
                    AssignedUser = user
                }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetAllWithDetailsAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _service.GetPerformanceReportAsync();

            // Assert
            Assert.Single(result);
            var userReport = result.First();
            Assert.Equal(1, userReport.TotalCompletedTasks);
            Assert.Equal(1.0/30, userReport.AverageCompletedTasksLast30Days, 2);
        }
    }
}
