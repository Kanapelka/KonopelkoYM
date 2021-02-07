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
using Task = Athena.Infrastructure.Models.Task;

namespace Athena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class TicketsController : Controller
    {
        private readonly TicketService _ticketService;


        public TicketsController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketAsync([FromRoute] int id)
        {
            Result<Ticket> result = await _ticketService.GetTicketByIdAsync(id);

            return result.ResultType switch
            {
                ResultType.NotFound => NotFound("Ticket not found"),
                ResultType.Ok => Ok(result.Payload),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTicketTasks([FromRoute] int id)
        {
            Result<IReadOnlyCollection<Task>> result = await _ticketService.GetTicketTasksAsync(id);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Payload),
                ResultType.NotFound => NotFound("Ticket not found"),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetTicketCommends([FromRoute] int id)
        {
            Result<IReadOnlyCollection<CommentModel>> result = await _ticketService.GetTicketComments(id);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Payload),
                ResultType.NotFound => NotFound("Ticket not found"),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrCreateTicketasync([FromBody] Ticket ticket)
        {
            Result<Ticket> result = await _ticketService.UpdateOrCreateTicketAsync(ticket);

            return result.ResultType switch
            {
                ResultType.Ok => StatusCode(StatusCodes.Status200OK, result.Payload),
                ResultType.NotFound => StatusCode(StatusCodes.Status404NotFound, result.Message),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}