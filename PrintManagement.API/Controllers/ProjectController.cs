using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.Constants;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Project;

namespace PrintManagement.API.Controllers
{

    [Route(Constant.DefaultRoute.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create(CreateProject_Request createProject_Request)
        {
            return Ok(await _projectService.Create(createProject_Request));
        }
    }
}
