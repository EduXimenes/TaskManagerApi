using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retorna todos os usuários
    /// </summary>
    /// <returns>Lista de usuários</returns>
    /// <response code="200">Retorna a lista de usuários</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserViewModel>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retorna um usuário específico pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado</returns>
    /// <response code="200">Retorna o usuário solicitado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserViewModel>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Cria um novo usuário
    /// Possíveis roles:
    /// Default = 1
    /// Manager = 2
    /// </summary>
    /// <param name="inputModel">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserViewModel>> Create(CreateUserInputModel inputModel)
    {
        try
        {
            var user = await _userService.CreateAsync(inputModel);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="inputModel">Dados atualizados do usuário</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Usuário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CreateUserInputModel inputModel)
    {
        try
        {
            await _userService.UpdateAsync(id, inputModel);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Usuário removido com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}