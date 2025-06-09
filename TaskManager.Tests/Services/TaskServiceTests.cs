using AutoMapper;
using Moq;
using TaskManager.Application.Mappings;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Common;
using Xunit;
using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new TaskItemProfile());
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();
            
            _taskService = new TaskService(_mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTaskAndHistory()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var inputModel = new CreateTaskInputModel
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Medium,
                UserId = userId,
                ProjectId = projectId
            };

            var task = new TaskItem(TaskPriority.Medium)
            {
                Title = inputModel.Title,
                Description = inputModel.Description,
                DueDate = inputModel.DueDate,
                Status = TaskStatusEnum.Pending,
                ProjectId = inputModel.ProjectId,
                Project = new Project { Name = "Test Project" },
                AssignedUser = new User { Name = "Test User" },
                TaskHistories = new List<TaskHistory>
                {
                    new TaskHistory
                    {
                        Description = "Tarefa criada",
                        TaskStatusEnum = TaskStatusEnum.Pending,
                        User = new User { Name = "Test User" }
                    }
                }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.CountByProjectIdAsync(projectId))
                .ReturnsAsync(0);

            _mockUnitOfWork.Setup(u => u.Tasks.AddAsync(It.IsAny<TaskItem>()))
                .Returns(Task.FromResult(task));

            _mockUnitOfWork.Setup(u => u.TasksHistories.AddAsync(It.IsAny<TaskHistory>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.CreateAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.Title, result.Title);
            Assert.Equal(inputModel.Description, result.Description);
            Assert.Equal(TaskStatusEnum.Pending, result.Status);
            Assert.Equal(inputModel.ProjectId, result.ProjectId);
            Assert.Equal("Test Project", result.ProjectName);
            Assert.Equal("Test User", result.UserName);
            Assert.Single(result.History);
            Assert.Equal("Tarefa criada", result.History.First().Description);

            _mockUnitOfWork.Verify(u => u.TasksHistories.AddAsync(It.Is<TaskHistory>(h => 
                h.Description == "Tarefa criada" && 
                h.TaskStatusEnum == TaskStatusEnum.Pending)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenProjectTaskLimitExceeded()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var inputModel = new CreateTaskInputModel
            {
                ProjectId = projectId
            };

            _mockUnitOfWork.Setup(u => u.Tasks.CountByProjectIdAsync(projectId))
                .ReturnsAsync(20);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _taskService.CreateAsync(inputModel));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem(TaskPriority.Medium)
            {
                Title = "Test Task",
                Project = new Project { Name = "Test Project" },
                AssignedUser = new User { Name = "Test User" }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.GetByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Title, result.Title);
            Assert.Equal("Test Project", result.ProjectName);
            Assert.Equal("Test User", result.UserName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskService.GetByIdAsync(taskId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            var existingTask = new TaskItem(TaskPriority.Medium)
            {
                Title = "Titulo Original",
                Description = "Descricao Original",
                DueDate = DateTime.UtcNow,
                Status = TaskStatusEnum.Pending,
                UserId = userId,
                ProjectId = projectId,
                Project = new Project { Name = "Test Project" },
                AssignedUser = new User { Name = "Test User" },
                TaskHistories = new List<TaskHistory>()
            };

            var inputModel = new UpdateTaskInputModel
            {
                Title = "Updated Title",
                Status = TaskStatusEnum.InProgress
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId))
                .ReturnsAsync(existingTask);

            _mockUnitOfWork.Setup(u => u.TasksHistories.AddAsync(It.IsAny<TaskHistory>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.UpdateAsync(taskId, inputModel);

            // Assert
            Assert.Equal("Updated Title", existingTask.Title);
            Assert.Equal("Descricao Original", existingTask.Description);
            Assert.Equal(TaskStatusEnum.InProgress, existingTask.Status);
            Assert.Equal(userId, existingTask.UserId);
            Assert.Equal(projectId, existingTask.ProjectId);

            _mockUnitOfWork.Verify(u => u.TasksHistories.AddAsync(It.Is<TaskHistory>(h => 
                h.Description.Contains("Título alterado") && 
                h.TaskStatusEnum == TaskStatusEnum.InProgress)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleStatusChangeToCompleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskItem(TaskPriority.Medium)
            {
                Status = TaskStatusEnum.InProgress,
                Project = new Project { Name = "Test Project" },
                AssignedUser = new User { Name = "Test User" },
                TaskHistories = new List<TaskHistory>()
            };

            var inputModel = new UpdateTaskInputModel
            {
                Status = TaskStatusEnum.Completed
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId))
                .ReturnsAsync(existingTask);

            _mockUnitOfWork.Setup(u => u.TasksHistories.AddAsync(It.IsAny<TaskHistory>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.UpdateAsync(taskId, inputModel);

            // Assert
            Assert.Equal(TaskStatusEnum.Completed, existingTask.Status);
            Assert.NotNull(existingTask.CompletedAt);

            _mockUnitOfWork.Verify(u => u.TasksHistories.AddAsync(It.Is<TaskHistory>(h => 
                h.Description.Contains("Tarefa marcada como concluída") && 
                h.TaskStatusEnum == TaskStatusEnum.Completed)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var inputModel = new UpdateTaskInputModel();

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _taskService.UpdateAsync(taskId, inputModel));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTask_WhenExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem(TaskPriority.Medium);

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            await _taskService.DeleteAsync(taskId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Tasks.DeleteAsync(taskId), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _taskService.DeleteAsync(taskId));
        }

        [Fact]
        public async Task GetByProjectIdAsync_ShouldReturnTasks()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItem(TaskPriority.Medium) { Title = "Task 1", Project = new Project { Name = "Project" } },
                new TaskItem(TaskPriority.Medium) { Title = "Task 2", Project = new Project { Name = "Project" } }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByProjectIdAsync(projectId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetByProjectIdAsync(projectId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.Equal("Project", t.ProjectName));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItem(TaskPriority.Medium) { Title = "Task 1", AssignedUser = new User { Name = "User" } },
                new TaskItem(TaskPriority.Medium) { Title = "Task 2", AssignedUser = new User { Name = "User" } }
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.Equal("User", t.UserName));
        }
    }
}
