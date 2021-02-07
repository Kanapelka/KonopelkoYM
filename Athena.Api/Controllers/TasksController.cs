using System.Threading.Tasks;
using Athena.Core.Result;
using Athena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketTask = Athena.Infrastructure.Models.Task;


namespace Athena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class TasksController : Controller
    {
        private readonly TaskSerivce _taskSerivce;


        public TasksController(TaskSerivce taskSerivce)
        {
            _taskSerivce = taskSerivce;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TicketTask task)
        {
            Result<TicketTask> result = await _taskSerivce.AddTaskAsync(task);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Payload),
                ResultType.NotFound => NotFound(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message),
            };
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TicketTask task)
        {
            Result<TicketTask> result = await _taskSerivce.ModifyTaskAsync(task);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Payload),
                ResultType.NotFound => NotFound(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message),
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CreateTaskAsync([FromRoute] int id)
        {
            Result<string> result = await _taskSerivce.DeleteTaskAsync(id);

            return result.ResultType switch
            {
                ResultType.Ok => Ok(result.Message),
                ResultType.NotFound => NotFound(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message),
            };
        }
    }
}