using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetByTaskId(Guid taskId)
        {
            var comments = await _commentService.GetByTaskIdAsync(taskId);
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentViewModel>> GetById(Guid id)
        {
            try
            {
                var comment = await _commentService.GetByIdAsync(id);
                return Ok(comment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CommentViewModel>> Create(CreateCommentInputModel inputModel)
        {
            var comment = await _commentService.CreateAsync(inputModel);
            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentViewModel>> Update(Guid id, CreateCommentInputModel inputModel)
        {
            try
            {
                await _commentService.UpdateAsync(id, inputModel);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _commentService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
