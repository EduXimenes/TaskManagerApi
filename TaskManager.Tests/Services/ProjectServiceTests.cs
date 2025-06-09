using Moq;
using AutoMapper;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.ViewModels;
using Xunit;
using TaskManager.Domain.InputModels;
using System.Linq.Expressions;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProjectService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProject()
        {
            // Arrange
            var inputModel = new CreateProjectInputModel
            {
                Name = "Test Project"
            };

            var project = new Project
            {
                Name = inputModel.Name
            };

            var projectViewModel = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name
            };

            _mockMapper.Setup(m => m.Map<Project>(inputModel))
                .Returns(project);
            _mockMapper.Setup(m => m.Map<ProjectViewModel>(It.IsAny<Project>()))
                .Returns(projectViewModel);

            // Setup mock for GetByIdWithUserAndTasksAsync
            _mockUnitOfWork.Setup(u => u.Projects.GetByIdWithUserAndTasksAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

            // Act
            var result = await _service.CreateAsync(inputModel);

            // Assert
            Assert.Equal(project.Id, result.Id);
            Assert.Equal(project.Name, result.Name);
            _mockUnitOfWork.Verify(u => u.Projects.AddAsync(project), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProject()
        {
            // Arrange
            var project = new Project
            {
                Name = "Old Name"
            };

            var inputModel = new CreateProjectInputModel
            {
                Name = "New Name"
            };

            var projectViewModel = new ProjectViewModel
            {
                Id = project.Id,
                Name = inputModel.Name
            };

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(project.Id))
                .ReturnsAsync(project);
            _mockMapper.Setup(m => m.Map(inputModel, project))
                .Returns(project);
            _mockMapper.Setup(m => m.Map<ProjectViewModel>(project))
                .Returns(projectViewModel);

            // Act
            await _service.UpdateAsync(project.Id, inputModel);

            // Assert
            _mockUnitOfWork.Verify(u => u.Projects.UpdateAsync(project), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenProjectNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var inputModel = new CreateProjectInputModel();

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateAsync(projectId, inputModel));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProject()
        {
            // Arrange
            var project = new Project
            {
                Name = "Test Project"
            };

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(project.Id))
                .ReturnsAsync(project);
            _mockUnitOfWork.Setup(u => u.Tasks.AnyAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                .ReturnsAsync(false);

            // Act
            await _service.DeleteAsync(project.Id);

            // Assert
            _mockUnitOfWork.Verify(u => u.Projects.DeleteAsync(project.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenProjectNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.DeleteAsync(projectId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Name = "Project 1", User = new User { Name = "User 1", Email = "user1@example.com" } },
                new Project { Name = "Project 2", User = new User { Name = "User 2", Email = "user2@example.com" } }
            };

            var projectViewModels = new List<ProjectViewModel>
            {
                new ProjectViewModel { Name = "Project 1", UserName = "User 1", UserEmail = "user1@example.com" },
                new ProjectViewModel { Name = "Project 2", UserName = "User 2", UserEmail = "user2@example.com" }
            };

            _mockUnitOfWork.Setup(u => u.Projects.GetAllWithUserAsync())
                .ReturnsAsync(projects);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProjectViewModel>>(projects))
                .Returns(projectViewModels);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.NotNull(p.Name));
            Assert.All(result, p => Assert.NotNull(p.UserName));
            Assert.All(result, p => Assert.NotNull(p.UserEmail));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProject_WhenExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Name = "Test Project",
                User = new User { Name = "Test User", Email = "test@example.com" },
                Tasks = new List<TaskItem>
                {
                    new TaskItem(TaskPriority.Medium) { Title = "Task 1" },
                    new TaskItem(TaskPriority.High) { Title = "Task 2" }
                }
            };

            var projectViewModel = new ProjectViewModel
            {
                Name = project.Name,
                UserName = project.User.Name,
                UserEmail = project.User.Email,
                TasksCount = project.Tasks.Count
            };

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdWithUserAndTasksAsync(projectId))
                .ReturnsAsync(project);
            _mockMapper.Setup(m => m.Map<ProjectViewModel>(project))
                .Returns(projectViewModel);

            // Act
            var result = await _service.GetByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(project.Name, result.Name);
            Assert.Equal(project.User.Name, result.UserName);
            Assert.Equal(project.User.Email, result.UserEmail);
            Assert.Equal(project.Tasks.Count, result.TasksCount);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Projects.GetByIdWithUserAndTasksAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _service.GetByIdAsync(projectId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenProjectHasIncompleteTasks()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Name = "Test Project"
            };

            _mockUnitOfWork.Setup(u => u.Projects.GetByIdAsync(projectId))
                .ReturnsAsync(project);
            _mockUnitOfWork.Setup(u => u.Tasks.AnyAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.DeleteAsync(projectId));
        }
    }
}
