using System.Threading.Tasks;
using Athena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;


        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        [HttpGet("{id}/tickets")]
        public async Task<IActionResult> GetHighestPriorityTickets(int id)
        {
            return Ok(await _dashboardService.GetHighestPriorityTickets(id));
        }

        [HttpGet("{id}/teammates")]
        public async Task<IActionResult> GetRecentTeammates(int id)
        {
            return Ok(await _dashboardService.GetRecentTeammates(id));
        }
    }
}