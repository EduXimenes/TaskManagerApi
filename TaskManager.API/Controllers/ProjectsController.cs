using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.InputModels;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectViewModel>>> GetAll()
        => Ok(await _projectService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectViewModel>> GetById(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectViewModel>> Create(CreateProjectInputModel inputModel)
    {
        var projectId = await _projectService.CreateAsync(inputModel);
        var createdProject = await _projectService.GetByIdAsync(projectId);
        return CreatedAtAction(nameof(GetById), new { id = projectId }, createdProject);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateProjectInputModel inputModel)
    {
        await _projectService.UpdateAsync(id, inputModel);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }
}
