using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de projetos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retorna todos os projetos
    /// </summary>
    /// <returns>Lista de projetos</returns>
    /// <response code="200">Retorna a lista de projetos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectViewModel>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    /// <summary>
    /// Retorna um projeto específico pelo ID
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Projeto encontrado</returns>
    /// <response code="200">Retorna o projeto solicitado</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectViewModel>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
            return NotFound();

        return Ok(project);
    }

    /// <summary>
    /// Cria um novo projeto
    /// </summary>
    /// <param name="inputModel">Dados do projeto a ser criado</param>
    /// <returns>Projeto criado</returns>
    /// <response code="201">Projeto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectViewModel>> Create(CreateProjectInputModel inputModel)
    {
        try
        {
            var project = await _projectService.CreateAsync(inputModel);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um projeto existente
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <param name="inputModel">Dados atualizados do projeto</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Projeto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CreateProjectInputModel inputModel)
    {
        try
        {
            await _projectService.UpdateAsync(id, inputModel);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove um projeto
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Projeto removido com sucesso</response>
    /// <response code="404">Projeto não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
