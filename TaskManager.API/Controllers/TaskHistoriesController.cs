using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskHistoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskHistoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskHistory>>> GetAll()
    {
        return await _context.TaskHistories
            .Include(h => h.TaskItem)
            .Include(h => h.User)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskHistory>> GetById(Guid id)
    {
        var history = await _context.TaskHistories
            .Include(h => h.TaskItem)
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (history == null) return NotFound();
        return history;
    }

    [HttpPost]
    public async Task<ActionResult<TaskHistory>> Create(TaskHistory history)
    {
        _context.TaskHistories.Add(history);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = history.Id }, history);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, TaskHistory history)
    {
        if (id != history.Id) return BadRequest();

        _context.Entry(history).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var history = await _context.TaskHistories.FindAsync(id);
        if (history == null) return NotFound();

        _context.TaskHistories.Remove(history);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
