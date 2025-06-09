using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de tarefas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Retorna todas as tarefas
    /// </summary>
    /// <returns>Lista de tarefas</returns>
    /// <response code="200">Retorna a lista de tarefas</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskViewModel>>> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Retorna uma tarefa específica pelo ID
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <returns>Tarefa encontrada</returns>
    /// <response code="200">Retorna a tarefa solicitada</response>
    /// <response code="404">Tarefa não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskViewModel>> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    /// <summary>
    /// Retorna todas as tarefas de um projeto específico
    /// </summary>
    /// <param name="projectId">ID do projeto</param>
    /// <returns>Lista de tarefas do projeto</returns>
    /// <response code="200">Retorna a lista de tarefas do projeto</response>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(IEnumerable<TaskViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskViewModel>>> GetByProjectId(Guid projectId)
    {
        var tasks = await _taskService.GetByProjectIdAsync(projectId);
        return Ok(tasks);
    }

    /// <summary>
    /// Retorna todas as tarefas de um usuário específico
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Lista de tarefas do usuário</returns>
    /// <response code="200">Retorna a lista de tarefas do usuário</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<TaskViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskViewModel>>> GetByUserId(Guid userId)
    {
        var tasks = await _taskService.GetByUserIdAsync(userId);
        return Ok(tasks);
    }

    /// <summary>
    /// Cria uma nova tarefa.
    /// É necessário um projeto e um usuário para criar uma tarefa.
    /// </summary>
    /// <param name="inputModel">Dados da tarefa a ser criada</param>
    /// <returns>Tarefa criada</returns>
    /// <response code="201">Tarefa criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Projeto ou usuário não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskViewModel>> Create(CreateTaskInputModel inputModel)
    {
        try
        {
            var task = await _taskService.CreateAsync(inputModel);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza uma tarefa existente
    /// Possíveis status:
    /// Pending = 1
    /// InProgress = 2
    /// Completed = 3
    /// Cancelled = 4
    /// Possíveis prioridades:
    ///  Low = 1
    ///  Medium = 2
    ///  High = 3
    ///  Urgent = 4
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <param name="inputModel">Dados atualizados da tarefa</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Tarefa atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Tarefa não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateTaskInputModel inputModel)
    {
        try
        {
            await _taskService.UpdateAsync(id, inputModel);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove uma tarefa
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Tarefa removida com sucesso</response>
    /// <response code="404">Tarefa não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
