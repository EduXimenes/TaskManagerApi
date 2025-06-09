using AutoMapper;
using Moq;
using TaskManager.Application.Mappings;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();
            
            _userService = new UserService(_mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser()
        {
            // Arrange
            var inputModel = new CreateUserInputModel
            {
                Name = "Edu User",
                Email = "Edu@example.com",
                Role = UserRole.Default
            };

            var user = new User
            {
                Name = inputModel.Name,
                Email = inputModel.Email,
                Role = inputModel.Role
            };

            _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(user));

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.CreateAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.Name, result.Name);
            Assert.Equal(inputModel.Email, result.Email);
            Assert.Equal(inputModel.Role, result.Role);

            _mockUnitOfWork.Verify(u => u.Users.AddAsync(It.Is<User>(u => 
                u.Name == inputModel.Name && 
                u.Email == inputModel.Email &&
                u.Role == inputModel.Role)), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Name = "Edu User",
                Email = "Edu@example.com"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Name = "Edu 1", Email = "Edu1@example.com" },
                new User { Name = "Edu 2", Email = "Edu2@example.com" }
            };

            _mockUnitOfWork.Setup(u => u.Users.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, u => Assert.NotNull(u.Name));
            Assert.All(result, u => Assert.NotNull(u.Email));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Name = "Edu",
                Email = "Edu@example.com"
            };

            var inputModel = new CreateUserInputModel
            {
                Name = "EduUpdated ",
                Email = "EduUpdated@example.com"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            // Act
            await _userService.UpdateAsync(userId, inputModel);

            // Assert
            Assert.Equal(inputModel.Name, existingUser.Name);
            Assert.Equal(inputModel.Email, existingUser.Email);

            _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(existingUser), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var inputModel = new CreateUserInputModel();

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _userService.UpdateAsync(userId, inputModel));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser_WhenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User();

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(userId), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _userService.DeleteAsync(userId));
        }
    }
} 