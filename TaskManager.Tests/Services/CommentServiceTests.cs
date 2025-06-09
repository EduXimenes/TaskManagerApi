using Moq;
using AutoMapper;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.ViewModels;
using Xunit;
using TaskManager.Domain.InputModels;

namespace TaskManager.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new CommentService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateComment()
        {
            // Arrange
            var task = new TaskItem(TaskPriority.Medium);
            var user = new User { Name = "Test User" };

            var inputModel = new CreateCommentInputModel
            {
                TaskItemId = task.Id,
                UserId = user.Id,
                Content = "Test Comment"
            };

            var comment = new Comment
            {
                TaskItemId = inputModel.TaskItemId,
                UserId = inputModel.UserId,
                Content = inputModel.Content,
                User = user,
                TaskItem = task
            };

            var commentViewModel = new CommentViewModel
            {
                Id = comment.Id,
                UserName = user.Name,
                Content = comment.Content
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdAsync(task.Id))
                .ReturnsAsync(task);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(user.Id))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<Comment>(inputModel))
                .Returns(comment);
            _mockMapper.Setup(m => m.Map<CommentViewModel>(It.IsAny<Comment>()))
                .Returns(commentViewModel);

            // Setup mock for GetByIdWithDetailsAsync
            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(comment);

            // Act
            var result = await _service.CreateAsync(inputModel);

            // Assert
            Assert.Equal(comment.Id, result.Id);
            Assert.Equal(user.Name, result.UserName);
            Assert.Equal(comment.Content, result.Content);
            _mockUnitOfWork.Verify(u => u.Comments.AddAsync(It.IsAny<Comment>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var inputModel = new CreateCommentInputModel
            {
                TaskItemId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = "Test Comment"
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdAsync(inputModel.TaskItemId))
                .ReturnsAsync((TaskItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.CreateAsync(inputModel));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var task = new TaskItem(TaskPriority.Medium);
            var inputModel = new CreateCommentInputModel
            {
                TaskItemId = task.Id,
                UserId = Guid.NewGuid(),
                Content = "Test Comment"
            };

            _mockUnitOfWork.Setup(u => u.Tasks.GetByIdAsync(task.Id))
                .ReturnsAsync(task);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(inputModel.UserId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.CreateAsync(inputModel));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteComment()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Test Comment"
            };

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            // Act
            await _service.DeleteAsync(comment.Id);

            // Assert
            _mockUnitOfWork.Verify(u => u.Comments.DeleteAsync(comment.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdAsync(commentId))
                .ReturnsAsync((Comment)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.DeleteAsync(commentId));
        }
    }
}
