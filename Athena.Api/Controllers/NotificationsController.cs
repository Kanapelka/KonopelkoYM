using System.Threading.Tasks;
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
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;


        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpPut]
        public async Task<IActionResult> UpdateNotification([FromBody] Notification notification)
        {
            Result<Notification> result = await _notificationService.UpdateNotification(notification);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Payload),
                ResultType.NotFound => NotFound(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}