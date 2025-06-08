using AutoMapper;
using TaskManager.Domain.Entities;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;

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

        public async Task<CommentViewModel?> GetByIdAsync(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetByIdWithDetailsAsync(id);
            return comment == null ? null : _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> CreateAsync(CreateCommentInputModel inputModel)
        {
            var comment = _mapper.Map<Comment>(inputModel);
            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.CommitAsync();

            var createdComment = await _unitOfWork.Comments.GetByIdWithDetailsAsync(comment.Id);
            return _mapper.Map<CommentViewModel>(createdComment);
        }

        public async Task UpdateAsync(Guid id, CreateCommentInputModel inputModel)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException($"Comentário com id {id} não encontrado.");

            _mapper.Map(inputModel, comment);
            await _unitOfWork.Comments.UpdateAsync(comment);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException($"Comentário com id {id} não encontrado.");

            await _unitOfWork.Comments.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
