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

        [Fact]
        public async Task GetByTaskIdAsync_ShouldReturnComments()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var comments = new List<Comment>
            {
                new Comment { Content = "Comment 1", User = new User { Name = "User 1" } },
                new Comment { Content = "Comment 2", User = new User { Name = "User 2" } }
            };

            var commentViewModels = new List<CommentViewModel>
            {
                new CommentViewModel { Content = "Comment 1", UserName = "User 1" },
                new CommentViewModel { Content = "Comment 2", UserName = "User 2" }
            };

            _mockUnitOfWork.Setup(u => u.Comments.GetByTaskIdAsync(taskId))
                .ReturnsAsync(comments);
            _mockMapper.Setup(m => m.Map<IEnumerable<CommentViewModel>>(comments))
                .Returns(commentViewModels);

            // Act
            var result = await _service.GetByTaskIdAsync(taskId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, c => Assert.NotNull(c.Content));
            Assert.All(result, c => Assert.NotNull(c.UserName));
        }

        [Fact]
        public async Task GetByTaskIdAsync_ShouldReturnEmptyList_WhenNoComments()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var emptyComments = new List<Comment>();

            _mockUnitOfWork.Setup(u => u.Comments.GetByTaskIdAsync(taskId))
                .ReturnsAsync(emptyComments);
            _mockMapper.Setup(m => m.Map<IEnumerable<CommentViewModel>>(emptyComments))
                .Returns(new List<CommentViewModel>());

            // Act
            var result = await _service.GetByTaskIdAsync(taskId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnComment_WhenExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = new Comment
            {
                Content = "Test Comment",
                User = new User { Name = "Test User" }
            };

            var commentViewModel = new CommentViewModel
            {
                Content = comment.Content,
                UserName = comment.User.Name
            };

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(commentId))
                .ReturnsAsync(comment);
            _mockMapper.Setup(m => m.Map<CommentViewModel>(comment))
                .Returns(commentViewModel);

            // Act
            var result = await _service.GetByIdAsync(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(comment.Content, result.Content);
            Assert.Equal(comment.User.Name, result.UserName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(commentId))
                .ReturnsAsync((Comment)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetByIdAsync(commentId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateComment()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var existingComment = new Comment
            {
                Content = "Old Content",
                User = new User { Name = "Test User" }
            };

            var inputModel = new CreateCommentInputModel
            {
                Content = "Updated Content",
                TaskItemId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var updatedComment = new Comment
            {
                Content = inputModel.Content,
                User = existingComment.User
            };

            var commentViewModel = new CommentViewModel
            {
                Content = inputModel.Content,
                UserName = existingComment.User.Name
            };

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdAsync(commentId))
                .ReturnsAsync(existingComment);
            _mockMapper.Setup(m => m.Map(inputModel, existingComment))
                .Returns(updatedComment);
            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(commentId))
                .ReturnsAsync(updatedComment);
            _mockMapper.Setup(m => m.Map<CommentViewModel>(updatedComment))
                .Returns(commentViewModel);

            // Act
            var result = await _service.UpdateAsync(commentId, inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.Content, result.Content);
            Assert.Equal(existingComment.User.Name, result.UserName);
            _mockUnitOfWork.Verify(u => u.Comments.UpdateAsync(It.IsAny<Comment>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var inputModel = new CreateCommentInputModel();

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdAsync(commentId))
                .ReturnsAsync((Comment)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateAsync(commentId, inputModel));
        }

        [Fact]
        public async Task UpdateAsync_ShouldPreserveUserAndTask_WhenUpdating()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var existingUser = new User { Name = "Original User" };
            var existingTask = new TaskItem(TaskPriority.Medium);
            var existingComment = new Comment
            {
                Content = "Old Content",
                User = existingUser,
                TaskItem = existingTask,
                UserId = existingUser.Id,
                TaskItemId = existingTask.Id
            };

            var inputModel = new CreateCommentInputModel
            {
                Content = "Updated Content",
                TaskItemId = Guid.NewGuid(), // Different task ID
                UserId = Guid.NewGuid() // Different user ID
            };

            var updatedComment = new Comment
            {
                Content = inputModel.Content,
                User = existingUser,
                TaskItem = existingTask,
                UserId = existingUser.Id,
                TaskItemId = existingTask.Id
            };

            var commentViewModel = new CommentViewModel
            {
                Content = updatedComment.Content,
                UserName = existingUser.Name
            };

            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(commentId))
                .ReturnsAsync(existingComment);
            _mockMapper.Setup(m => m.Map(inputModel, existingComment))
                .Callback<CreateCommentInputModel, Comment>((input, comment) => {
                    comment.Content = input.Content;
                });
            _mockMapper.Setup(m => m.Map<CommentViewModel>(It.IsAny<Comment>()))
                .Returns(commentViewModel);

            // Act
            var result = await _service.UpdateAsync(commentId, inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.Content, result.Content);
            Assert.Equal(existingUser.Name, result.UserName);
            Assert.Equal(existingComment.UserId, updatedComment.UserId);
            Assert.Equal(existingComment.TaskItemId, updatedComment.TaskItemId);
            _mockUnitOfWork.Verify(u => u.Comments.UpdateAsync(It.IsAny<Comment>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetCreatedAt_WhenCreatingComment()
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
            _mockUnitOfWork.Setup(u => u.Comments.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(comment);

            // Act
            var result = await _service.CreateAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(comment.Content, result.Content);
            Assert.Equal(user.Name, result.UserName);
            _mockUnitOfWork.Verify(u => u.Comments.AddAsync(It.Is<Comment>(c => 
                c.CreatedAt != default(DateTime))), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
