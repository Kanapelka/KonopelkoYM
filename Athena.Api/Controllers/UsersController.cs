using System.Collections.Generic;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Core.Services;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;


        public UsersController(UserService userService)
        {
            _userService = userService;
        }


        // GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            UserProfile user = await _userService.GetUserProfile(id);

            if (user == null) {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // PUT api/users
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfile userProfile)
        {
            Result<UserProfile> updateUserResult = await _userService.UpdateUser(userProfile);

            if (updateUserResult.ResultType == ResultType.NotFound) {
                return NotFound(updateUserResult.Message);
            }

            return Ok(updateUserResult.Payload);
        }

        // GET api/users/{id}/projects
        [HttpGet("{id}/projects")]
        public async Task<IActionResult> GetUserProjectsAsync(int id, [FromQuery(Name = "filter")] string porjectName, [FromQuery] int offset = 0, [FromQuery] int count = 10)
        {
            IReadOnlyCollection<ProjectThumbnail> result;

            if (porjectName != null) {
                result = await _userService.GetUserProjectsByNameAsync(id, porjectName, count, offset);
            }
            else {
                result = await _userService.GetUserProjectsAsync(id, offset, count);
            }

            return Ok(result);
        }

        // GET api/users/{id}/tickets
        [HttpGet("{id}/tickets")]
        public async Task<IActionResult> GetTicketsAssignedToUserAsync(int id, [FromQuery] int offset = 0, [FromQuery] int count = 10)
        {
            IReadOnlyCollection<TicketThumbnail> tickets = await _userService.GetTicketsAssignedThumbnailsAsync(id, count, offset);

            return Ok(tickets);
        }

        // GET api/users/{userId}/projects/{projectId}/tickets
        [HttpGet("{userId}/projects/{projectId}/tickets")]
        public async Task<IActionResult> GetTicketsAssignedToUserAsync(int userId, int projectId)
        {
            IReadOnlyCollection<TicketThumbnail> tickets =
                await _userService.GetTicketsAssignedThumbnailsAsync(userId, projectId);

            return Ok(tickets);
        }

        // GET api/users/{id}/notifications/new
        [HttpGet("{id}/notifications/new")]
        public async Task<IActionResult> GetNotifications(int id, [FromQuery] int offset = 0, [FromQuery] int count = 150)
        {
            IReadOnlyCollection<Notification> notifications = await _userService.GetNewUserNotifications(id, offset, count);

            return Ok(notifications);
        }

        // GET api/users/{id}/notifications
        [HttpGet("{id}/notifications")]
        public async Task<IActionResult> GetNewNotifications(int id, [FromQuery] int offset = 0, [FromQuery] int count = 150)
        {
            IReadOnlyCollection<Notification> notifications = await _userService.GetUserNotifications(id, offset, count);

            return Ok(notifications);
        }
    }
}