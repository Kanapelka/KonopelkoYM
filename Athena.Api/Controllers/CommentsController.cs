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
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;


        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comment comment)
        {
            Result<CommentModel> result = await _commentService.AddCommentAsync(comment);

            return result.ResultType switch
            {
                ResultType.Created => StatusCode(StatusCodes.Status201Created, result.Payload),
                ResultType.NotFound => NotFound(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message),
            };
        }
    }
}