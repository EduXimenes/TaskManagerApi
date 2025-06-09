using Moq;
using AutoMapper;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.ViewModels;
using Xunit;
using TaskManager.Domain.InputModels;
using System.Linq.Expressions;

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
    }
}
