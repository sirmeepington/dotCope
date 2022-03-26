using DotCope.Coping;
using Microsoft.AspNetCore.Mvc;

namespace DotCope
{
    [Route("/")]
    [ApiController]
    public class CopeController : ControllerBase
    {
        private readonly CopeService _copeService;
        private static readonly Random _random = new();

        public CopeController(CopeService copeService)
        {
            _copeService = copeService;
        }

        [HttpGet]
        public IActionResult Cope() => Redirect(_random.Next().ToString());

        [HttpGet("/{seed}")]
        public async Task<IActionResult> CopeSeeded(int seed) => new FileStreamResult(await _copeService.CreateRandomCope(seed), "image/gif");
    }
}
