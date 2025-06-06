//using Microsoft.AspNetCore.Mvc;

//namespace TaskManager.API.Controllers
//{
//    [Route("api/task-management")]
//    [ApiController]
//    public class ProjectsController : ControllerBase
//    {
//        private readonly IProjectsService _projectsService;

//        public ProjectsController(IProjectsService projectsService)
//        {
//            projectsService = _projectsService;
//        }

//        [HttpGet("/GetAllProjects")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<IActionResult> GetAllProjects()
//        {
//            var projects = await _projectsService.GetAllProjects();
//            return Ok(projects);
//        }

//        [HttpGet("/GetProject/{idProject}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> GetProjectById(Guid idProject)
//        {
//            var project = await _projectsService.GetProject(idProject);

//            if (project == null)
//            {
//                return NotFound();
//            }
//            var viewModel = _mapper.Map<ProjectViewModel>(project);

//            return Ok(viewModel);
//        }
//        [HttpPost("CreateProject/")]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        public async Task<IActionResult> PostProject(ProjectInputModel input)
//        {
//            var project = _mapper.Map<Project>(input);
//            await _projectsService.AddProject(project);
//            var result = _mapper.Map<ProjectViewModel>(project);
//            return Created("New Project", result);
//        }
//        [HttpDelete("DeleteProject/{idProject}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> DeleteProject(Guid idProject)
//        {
//            var project = await _context.GetProject(idProject);
//            if (project == null)
//            {
//                return NotFound();
//            }
//            var activeTasks = project.Tasks?
//                .Any(ts => ts.Status != TaskStatusCode.Concluida);
//            if (activeTasks != false)
//                return BadRequest("Existem tarefas pendentes neste projeto, conclua ou encerre as tarefas antes de remover o projeto.");

//            await _context.DeleteProject(idProject);

//            return NoContent();

//        }
//    }
//}
