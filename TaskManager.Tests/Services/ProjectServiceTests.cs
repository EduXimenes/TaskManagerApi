using Moq;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Xunit;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateProjectAsync_ShouldCreateProject_WhenUserExists()
    {
        // Arrange
        var user = new User();
        var userId = user.Id;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var mockProjectRepo = new Mock<IRepository<Project>>();
        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);

        var service = new ProjectService(mockUnitOfWork.Object);

        // Act
        var result = await service.CreateProjectAsync(userId, "Projeto Teste");

        // Assert
        mockProjectRepo.Verify(p => p.AddAsync(It.IsAny<Project>()), Times.Once);
        mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldThrowException_WhenUserNotFound()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User)null);

        var service = new ProjectService(mockUnitOfWork.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.CreateProjectAsync(Guid.NewGuid(), "Teste"));
    }
}
