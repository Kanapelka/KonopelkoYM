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
    [Route("api/statuses")]
    [EnableCors("CorsPolicy")]
    public class TicketStatusesController : Controller
    {
        private readonly TicketStatusService _ticketStatusService;


        public TicketStatusesController(TicketStatusService ticketStatusService)
        {
            _ticketStatusService = ticketStatusService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateNewStatusAsync([FromBody] TicketStatus status)
        {
            Result<TicketStatus> result = await _ticketStatusService.AddNewStatusAsync(status);

            return result.ResultType switch
            {
                ResultType.Created => Ok(result.Payload),
                ResultType.Forbidden => StatusCode(403, result),
                _ => StatusCode(500, result)
            };
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] TicketStatus ticketStatus)
        {
            Result<TicketStatus> result = await _ticketStatusService.UpdateStatusAsync(ticketStatus);

            return result.ResultType switch
            {
                ResultType.Created => Ok(result.Payload),
                ResultType.Forbidden => StatusCode(403, result),
                _ => StatusCode(500, result)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketStatusAsync([FromRoute(Name = "id")] int statusId)
        {
            Result<string> result = await _ticketStatusService.DeleteStatus(statusId);

            return result.ResultType switch
            {
                ResultType.Deleted => StatusCode(StatusCodes.Status200OK, result),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}