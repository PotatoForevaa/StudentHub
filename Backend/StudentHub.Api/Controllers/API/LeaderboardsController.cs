using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.Extensions;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/leaderboards")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly IProjectUseCase _projectUseCase;

        public LeaderboardsController(IProjectUseCase projectUseCase)
        {
            _projectUseCase = projectUseCase;
        }

        [HttpGet("{type}/{period}")]
        public async Task<IActionResult> GetLeaderboard([FromRoute] string type, [FromRoute] string period, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var result = await _projectUseCase.GetLeaderboardAsync(type, period, page, size);
            return result.ToActionResult();
        }
    }
}
