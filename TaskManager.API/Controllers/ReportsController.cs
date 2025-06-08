using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("performance")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetPerformanceReport()
    {
        var sinceDate = DateTime.UtcNow.AddDays(-30);

        var userTaskGroups = await _context.Tasks
            .Where(t => t.Status == TaskStatus.Completed && t.DueDate >= sinceDate)
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
}
