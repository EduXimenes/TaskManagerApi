//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TaskManager.Domain.Entities;
//using TaskManager.Infrastructure.Persistence;

//namespace TaskManager.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class TaskHistoriesController : ControllerBase
//{
//    private readonly ITaskHistoriyService _context;

//    public TaskHistoriesController(AppDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<TaskHistory>>> GetAll()
//    {
//        return await _context.TaskHistories
//            .Include(h => h.TaskItem)
//            .Include(h => h.User)
//            .ToListAsync();
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<TaskHistory>> GetById(Guid id)
//    {
//        var history = await _context.TaskHistories
//            .Include(h => h.TaskItem)
//            .Include(h => h.User)
//            .FirstOrDefaultAsync(h => h.Id == id);

//        if (history == null) return NotFound();
//        return history;
//    }
//}
