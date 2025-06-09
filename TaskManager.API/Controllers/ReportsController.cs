using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IUserService _userService;

    public ReportsController(IReportService reportService, IUserService userService)
    {
        _reportService = reportService;
        _userService = userService;
    }

    [HttpGet("performance")]
    public async Task<IActionResult> GetPerformanceReport([FromQuery] Guid userId)
    {
        if (!await _userService.IsManagerAsync(userId))
            return Unauthorized("Apenas gerentes podem acessar este relatório.");

        var report = await _reportService.GetPerformanceReportAsync();
        return Ok(report);
    }

    [HttpGet("completed-tasks")]
    public async Task<IActionResult> GetCompletedTasksByUser([FromQuery] Guid userId, [FromQuery] DateTime sinceDate)
    {
        if (!await _userService.IsManagerAsync(userId))
            return Unauthorized("Apenas gerentes podem acessar este relatório.");

        var tasks = await _reportService.GetCompletedTasksByUserAsync(sinceDate);
        return Ok(tasks);
    }
}
