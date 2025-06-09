using AutoMapper;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Common;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Enums;

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
            var tasks = await _unitOfWork.Tasks.GetAllWithDetailsAsync();
            var completedTasks = tasks
                .Where(t => t.Status == TaskStatusEnum.Completed && 
                           t.CompletedAt >= sinceDate)
                .OrderByDescending(t => t.CompletedAt)
                .ToList();

            return _mapper.Map<IEnumerable<TaskViewModel>>(completedTasks);
        }

        public async Task<IEnumerable<PerformanceReportViewModel>> GetPerformanceReportAsync()
        {
            var sinceDate = DateTime.UtcNow.AddDays(-30);
            var tasks = await _unitOfWork.Tasks.GetAllWithDetailsAsync();
            
            var userPerformance = tasks
                .Where(t => t.Status == TaskStatusEnum.Completed && 
                           t.CompletedAt >= sinceDate)
                .GroupBy(t => new { t.AssignedUser.Id, t.AssignedUser.Name })
                .Select(g => new PerformanceReportViewModel
                {
                    UserId = g.Key.Id,
                    UserName = g.Key.Name,
                    TotalCompletedTasks = g.Count(),
                    AverageCompletedTasksLast30Days = g.Count() / 30.0
                })
                .OrderByDescending(p => p.TotalCompletedTasks)
                .ToList();

            return userPerformance;
        }
    }
}
