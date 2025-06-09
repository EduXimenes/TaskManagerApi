using AutoMapper;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserViewModel>>(users);
        }

        public async Task<UserViewModel?> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel> CreateAsync(CreateUserInputModel inputModel)
        {
            var user = _mapper.Map<User>(inputModel);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var createdUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
            return _mapper.Map<UserViewModel>(createdUser);
        }

        public async Task UpdateAsync(Guid id, CreateUserInputModel inputModel)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuário com id {id} não encontrado.");
            }
            _mapper.Map(inputModel, user);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuário com id {id} não encontrado.");
            }
            await _unitOfWork.Users.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> IsManagerAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user?.Role == UserRole.Manager;
        }
    }
} 