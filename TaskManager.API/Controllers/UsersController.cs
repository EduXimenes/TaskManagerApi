using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.InputModels;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserViewModel>> GetAll()
        {
            var viewModel = _context.Users.ToList();
            return Ok(_mapper.Map<IEnumerable<UserViewModel>>(viewModel));
        }

        [HttpGet("{id}")]
        public ActionResult<UserViewModel> GetById(Guid id)
        {
            var viewModel = _context.Users.Find(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserViewModel>(viewModel));
        }

        [HttpPost]
        public ActionResult<UserViewModel> Create(CreateUserInputModel inputModel)
        {
            var user = _mapper.Map<User>(inputModel);
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = _mapper.Map<UserViewModel>(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, viewModel);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, CreateUserInputModel inputModel)
        {
            var existing = _context.Users.Find(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = inputModel.Name;
            existing.Email = inputModel.Email;
            existing.Role = inputModel.Role;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}