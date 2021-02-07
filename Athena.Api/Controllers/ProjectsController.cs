using System.Collections.Generic;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Core.Services;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;


        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectSettingsAsync([FromRoute(Name = "id")] int projectId)
        {
            Result<ProjectThumbnail> result = await _projectService.GetProjectThumbnailAsync(projectId);

            switch (result.ResultType) {
                case ResultType.NotFound:
                    return NotFound(result.Message);
                case ResultType.Forbidden:
                    return StatusCode(403, result.Message);
                default:
                    return Ok(result.Payload);
            }
        }

        // GET api/projects/{id}/members
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetProjectMembersAsync([FromRoute(Name = "id")]int projectId)
        {
            IReadOnlyCollection<ProjectMember> users = await _projectService.GetProjectUsers(projectId);

            return Ok(users);
        }

        // POST api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectInfo project)
        {
            Result<Project> createdProject = await _projectService.CreateProjectAsync(project.ProjectName, project.OwnerId);
            _projectService.UpdateOwner(createdProject.Payload.ProjectId);

            return Ok(createdProject.Payload);
        }

        // PUT api/projects
        [HttpPut]
        public async Task<IActionResult> UpdateProjectAsync([FromBody] Project project)
        {
            Result<ProjectThumbnail> result = await _projectService.UpdateProjectAsync(project);

            return result.ResultType switch
            {
                ResultType.NotFound => NotFound(result.Message),
                ResultType.Forbidden => StatusCode(403, result.Message),
                _ => Ok(result.Payload)
            };
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMemberAsync([FromQuery(Name = "email")] string emailAddress, [FromRoute] int id)
        {
            Result<Member> result = await _projectService.CreateMemberAsync(emailAddress, id);

            return result.ResultType switch
            {
                ResultType.NotFound => NotFound(result.Message),
                ResultType.Forbidden => StatusCode(403, result.Message),
                ResultType.Bad => BadRequest(result),
                _ => Ok(result.Payload)
            };
        }

        [HttpGet("{id}/tickets")]
        public async Task<IActionResult> GetProjectTicketsAsync([FromRoute(Name = "id")] int projectId)
        {
            Result<IReadOnlyCollection<TicketThumbnail>> result = await _projectService.GetProjectTicketThumbnailsAsync(projectId);

            return result.ResultType switch
            {
                ResultType.NotFound => NotFound(result.Message),
                ResultType.Forbidden => StatusCode(403, result.Message),
                ResultType.Bad => BadRequest(result),
                _ => Ok(result.Payload)
            };
        }

        [HttpGet("{id}/statuses")]
        public async Task<IActionResult> GetTicketStatusesAsync([FromRoute(Name = "id")] int projectId)
        {
            Result<IReadOnlyCollection<TicketStatus>> result = await _projectService.GetStatusAvaibleAsync(projectId);

            return result.ResultType switch
            {
                ResultType.NotFound => NotFound(result.Message),
                ResultType.Forbidden => StatusCode(403, result.Message),
                ResultType.Bad => BadRequest(result),
                _ => Ok(result.Payload)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectAsync([FromRoute] int id)
        {
            Result<string> result = await _projectService.DeleteProjectAsync(id);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.Message),
                ResultType.NotFound => StatusCode(StatusCodes.Status404NotFound, result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message)
            };
        }
    }
}