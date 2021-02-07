using System.Threading.Tasks;
using Athena.Api.Services;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Core.Services;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class AuthorizationController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;
        private readonly JwtService _jwtService;


        public AuthorizationController(AuthorizationService authorizationService, JwtService jwtService)
        {
            _authorizationService = authorizationService;
            _jwtService = jwtService;
        }


        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] User user)
        {
            Result<int> authorizationResult
                = await _authorizationService.AuthorizeAsync(user);

            if (authorizationResult.ResultType == ResultType.Bad) {
                return BadRequest(authorizationResult.Message);
            }

            string token = _jwtService.CreateToken(authorizationResult.Payload, user.EmailAddress);

            UserInfo userInfo = new UserInfo
            {
                UserId = authorizationResult.Payload,
                Token = token
            };

            return Ok(userInfo);
        }

        [HttpGet]
        [Route("sign-in")]
        public async Task<IActionResult> SignInAsync([FromQuery(Name = "email")] string emailAddress, string password)
        {
            Result<int> authenticationResult =
                await _authorizationService.AuthenticateAsync(emailAddress, password);

            if (authenticationResult.ResultType == ResultType.NotFound) {
                return NotFound("There is no such a user!");
            }

            string tocken = _jwtService.CreateToken(authenticationResult.Payload, emailAddress);

            UserInfo userInfo = new UserInfo
            {
                UserId =authenticationResult.Payload,
                Token = tocken
            };

            return Ok(userInfo);
        }

        [HttpPost]
        [Route("sign-in/google")]
        public async Task<ActionResult<UserInfo>> SignInWithGoogle([FromBody] UserProfile userProfile)
        {
            int userId = await _authorizationService.AuthorizeWithGoogle(userProfile);

            string tocketn = _jwtService.CreateToken(userId, userProfile.EmailAddress);

            var userInfo = new UserInfo{ UserId = userId, Token = tocketn };
            return Ok(userInfo);
        }
    }
}