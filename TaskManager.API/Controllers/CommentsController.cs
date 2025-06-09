using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.ViewModels;

namespace TaskManager.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de comentários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Retorna todos os comentários de uma tarefa específica
        /// </summary>
        /// <param name="taskId">ID da tarefa</param>
        /// <returns>Lista de comentários da tarefa</returns>
        /// <response code="200">Retorna a lista de comentários</response>
        [HttpGet("task/{taskId}")]
        [ProducesResponseType(typeof(IEnumerable<CommentViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetByTaskId(Guid taskId)
        {
            var comments = await _commentService.GetByTaskIdAsync(taskId);
            return Ok(comments);
        }

        /// <summary>
        /// Retorna um comentário específico pelo ID
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <returns>Comentário encontrado</returns>
        /// <response code="200">Retorna o comentário solicitado</response>
        /// <response code="404">Comentário não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Cria um novo comentário
        /// </summary>
        /// <param name="inputModel">Dados do comentário a ser criado</param>
        /// <returns>Comentário criado</returns>
        /// <response code="201">Comentário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommentViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentViewModel>> Create(CreateCommentInputModel inputModel)
        {
            var comment = await _commentService.CreateAsync(inputModel);
            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
        }

        /// <summary>
        /// Atualiza um comentário existente
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <param name="inputModel">Dados atualizados do comentário</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Comentário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Comentário não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Remove um comentário
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Comentário removido com sucesso</response>
        /// <response code="404">Comentário não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
