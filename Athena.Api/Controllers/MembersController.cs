using System.Threading.Tasks;
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
    public class MembersController : Controller
    {
        private readonly MemberService _memberService;


        public MembersController(MemberService memberService)
        {
            _memberService = memberService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] Member member)
        {
            Result<Member> result = await _memberService.CreateMember(member);

            switch (result.ResultType) {
                case ResultType.Ok: return Ok(result);
                case ResultType.Forbidden: return StatusCode(403);
                case ResultType.NotFound: return NotFound(result);
                default: return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] Member member)
        {
            Result<string> result = await _memberService.UpdateRole(member.MemberId, member.Role);

            switch (result.ResultType) {
                case ResultType.Ok: return Ok(result);
                case ResultType.Forbidden: return StatusCode(403);
                case ResultType.NotFound: return NotFound(result);
                default: return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember([FromRoute] int id)
        {
            Result<string> result = await _memberService.DeleteMember(id);

            switch (result.ResultType) {
                case ResultType.Deleted: return Ok(result);
                case ResultType.Forbidden: return StatusCode(403);
                case ResultType.NotFound: return NotFound(result);
                default: return BadRequest();
            }
        }
    }
}