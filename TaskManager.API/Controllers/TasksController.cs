using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Application.InputModels;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TasksController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskViewModel>>> GetAll()
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<TaskViewModel>>(tasks));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskViewModel>> GetById(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments)
            .Include(t => t.TaskHistories)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }
            return _mapper.Map< TaskViewModel>(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskViewModel>> Create(CreateTaskInputModel inputModel)
    {
        var task = _mapper.Map<TaskItem>(inputModel);

        var taskCount = await _context.Tasks.CountAsync(t => t.ProjectId == inputModel.ProjectId);
        if (taskCount >= 20)
        {
            return BadRequest("Limite de 20 tarefas por projeto atingido.");
        }

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var viewModel = _mapper.Map<TaskViewModel>(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, viewModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskInputModel inputModel)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        task.Title = inputModel.Title;
        task.Description = inputModel.Description;
        task.DueDate = inputModel.DueDate;
        task.Status = inputModel.Status;

        var history = new TaskHistory
        {
            TaskItemId = task.Id,
            UserId = inputModel.UserId,
            Description = $"Tarefa atualizada: {task.Title}",
            ChangeDate = DateTime.UtcNow
        };
        _context.TaskHistories.Add(history);

        await _context.SaveChangesAsync();

        var result = _mapper.Map<TaskViewModel>(task);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
