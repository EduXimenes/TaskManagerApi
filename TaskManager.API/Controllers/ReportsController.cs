using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Interfaces.Services;

/// <summary>
/// Controlador responsável pelo gerenciamento de relatórios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IUserService _userService;

    public ReportsController(IReportService reportService, IUserService userService)
    {
        _reportService = reportService;
        _userService = userService;
    }

    /// <summary>
    /// Retorna o relatório de performance dos usuários
    /// </summary>
    /// <param name="userId">ID do usuário gerente que está solicitando o relatório</param>
    /// <returns>Relatório de performance</returns>
    /// <response code="200">Retorna o relatório de performance</response>
    /// <response code="401">Usuário não tem permissão para acessar o relatório</response>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPerformanceReport([FromQuery] Guid userId)
    {
        if (!await _userService.IsManagerAsync(userId))
            return Unauthorized("Apenas gerentes podem acessar este relatório.");

        var report = await _reportService.GetPerformanceReportAsync();
        return Ok(report);
    }

    /// <summary>
    /// Retorna as tarefas concluídas por usuário em um período específico
    /// </summary>
    /// <param name="userId">ID do usuário gerente que está solicitando o relatório</param>
    /// <param name="sinceDate">Data inicial para o filtro do relatório</param>
    /// <returns>Lista de tarefas concluídas</returns>
    /// <response code="200">Retorna a lista de tarefas concluídas</response>
    /// <response code="401">Usuário não tem permissão para acessar o relatório</response>
    [HttpGet("completed-tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCompletedTasksByUser([FromQuery] Guid userId, [FromQuery] DateTime sinceDate)
    {
        if (!await _userService.IsManagerAsync(userId))
            return Unauthorized("Apenas gerentes podem acessar este relatório.");

        var tasks = await _reportService.GetCompletedTasksByUserAsync(sinceDate);
        return Ok(tasks);
    }
}
