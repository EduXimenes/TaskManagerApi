using AutoMapper;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskViewModel>> GetCompletedTasksByUserAsync(DateTime sinceDate)
        {
            var tasks = await _unitOfWork.Reports.GetCompletedTasksByUserAsync(sinceDate);
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersWithCompletedTasksAsync(IEnumerable<Guid> userIds)
        {
            var users = await _unitOfWork.Reports.GetUsersWithCompletedTasksAsync(userIds);
            return _mapper.Map<IEnumerable<UserViewModel>>(users);
        }
    }
}
