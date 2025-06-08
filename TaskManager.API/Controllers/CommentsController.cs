using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.InputModels;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CommentsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(CreateCommentInputModel model)
    {
        var comment = _mapper.Map<Comment>(model);
        _context.Comments.Add(comment);

        var history = new TaskHistory
        {
            TaskItemId = model.TaskItemId,
            UserId = model.UserId,
            Description = $"Comentário adicionado: \"{model.Content}\"",
            ChangeDate = DateTime.UtcNow
        };
        _context.TaskHistories.Add(history);

        await _context.SaveChangesAsync();

        var result = _mapper.Map<CommentViewModel>(comment);
        return Ok(result);
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetCommentsByTask(Guid taskId)
    {
        var comments = await _context.Comments
            .Where(c => c.TaskItemId == taskId)
            .ToListAsync();

        var result = _mapper.Map<List<CommentViewModel>>(comments);
        return Ok(result);
    }
}
