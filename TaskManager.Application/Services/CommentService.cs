using AutoMapper;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.InputModels;

namespace TaskManager.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentViewModel>> GetByTaskIdAsync(Guid taskId)
        {
            var comments = await _unitOfWork.Comments.GetByTaskIdAsync(taskId);
            return _mapper.Map<IEnumerable<CommentViewModel>>(comments);
        }

        public async Task<CommentViewModel> GetByIdAsync(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetByIdWithDetailsAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found.");
            }

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> CreateAsync(CreateCommentInputModel inputModel)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(inputModel.TaskItemId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {inputModel.TaskItemId} not found.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(inputModel.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {inputModel.UserId} not found.");
            }
            var comment = _mapper.Map<Comment>(inputModel);
            comment.User = user;
            comment.TaskItem = task;

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(comment.Id);
        }

        public async Task<CommentViewModel> UpdateAsync(Guid id, CreateCommentInputModel inputModel)
        {
            var comment = await _unitOfWork.Comments.GetByIdWithDetailsAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found.");
            }
            _mapper.Map(inputModel, comment);
            comment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Comments.UpdateAsync(comment);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found.");
            }
            await _unitOfWork.Comments.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
