using Moq;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using Xunit;

public class CommentServiceTests
{
    [Fact]
    public async Task AddCommentAsync_ShouldAddCommentAndHistory()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var commentContent = "Novo comentário";

        var mockCommentRepo = new Mock<IRepository<Comment>>();
        var mockHistoryRepo = new Mock<IRepository<TaskHistory>>();
        var mockTaskRepo = new Mock<IRepository<TaskItem>>();
        var mockUow = new Mock<IUnitOfWork>();

        mockUow.Setup(u => u.Comments).Returns(mockCommentRepo.Object);
        mockUow.Setup(u => u.TaskHistories).Returns(mockHistoryRepo.Object);
        mockUow.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(new TaskItem(TaskPriority.Medium));

        var service = new CommentService(mockUow.Object);

        // Act
        await service.AddCommentAsync(taskId, userId, commentContent);

        // Assert
        mockCommentRepo.Verify(c => c.AddAsync(It.Is<Comment>(c =>
            c.TaskItemId == taskId && c.UserId == userId && c.Content == commentContent)), Times.Once);

        mockHistoryRepo.Verify(h => h.AddAsync(It.Is<TaskHistory>(h =>
            h.TaskItemId == taskId && h.UserId == userId && h.Description.Contains("comentário"))), Times.Once);

        mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }
}
