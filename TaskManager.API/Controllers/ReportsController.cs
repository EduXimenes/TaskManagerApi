using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;
using TaskManager.Infrastructure.Persistence;
using TaskStatusEnum = TaskManager.Domain.Enums.TaskStatusEnum;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IReportService _reportService;

    public ReportsController(AppDbContext context, IReportService reportService)
    {
        _context = context;
        _reportService = reportService;
    }

    [HttpGet("performance")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetPerformanceReport()
    {
        var sinceDate = DateTime.UtcNow.AddDays(-30);

        var userTaskGroups = await _context.Tasks
            .Where(t => t.Status == TaskStatusEnum.Completed && t.DueDate >= sinceDate)
            .GroupBy(t => t.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                CompletedCount = g.Count()
            })
            .ToListAsync();

        var users = await _context.Users
            .Where(u => userTaskGroups.Select(utg => utg.UserId).Contains(u.Id))
            .ToListAsync();

        var reports = users.Select(u =>
        {
            var completed = userTaskGroups.First(utg => utg.UserId == u.Id).CompletedCount;
            double average = completed / 30.0;
            return new PerformanceReportViewModel
            {
                UserId = u.Id,
                UserName = u.Name,
                AverageCompletedTasksLast30Days = average
            };
        }).ToList();

        return Ok(reports);
    }

    [HttpGet("completed-tasks")]
    public async Task<ActionResult<IEnumerable<TaskViewModel>>> GetCompletedTasksByUser([FromQuery] DateTime sinceDate)
    {
        var tasks = await _reportService.GetCompletedTasksByUserAsync(sinceDate);
        return Ok(tasks);
    }

    [HttpGet("users-with-completed-tasks")]
    public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsersWithCompletedTasks([FromQuery] Guid[] userIds)
    {
        var users = await _reportService.GetUsersWithCompletedTasksAsync(userIds);
        return Ok(users);
    }
}
