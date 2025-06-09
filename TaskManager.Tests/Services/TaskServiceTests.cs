using AutoMapper;
using Moq;
using TaskManager.Application.Mappings;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Common;
using Xunit;

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
    }
}
