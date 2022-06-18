using DotCope.Coping;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace DotCope
{
    [Route("/")]
    [ApiController]
    public class CopeController : ControllerBase
    {
        private readonly CopeService _copeService;
        private static readonly Random _random = new();
        private readonly IHub _sentryHub;

        public CopeController(CopeService copeService, IHub sentryHub)
        {
            _copeService = copeService;
            _sentryHub = sentryHub;
        }

        [HttpGet]
        public IActionResult Cope()
        {
            var span = _sentryHub.GetSpan()?.StartChild("pre-seed redirect");
            var result = Redirect(_random.Next().ToString());
            span?.Finish(SpanStatus.Ok);
            return result;
        }

        [HttpGet("/{seed}")]
        public async Task<IActionResult> CopeSeeded(int seed)
        {
            var span = _sentryHub.GetSpan()?.StartChild("seeded generation");
            var result = new FileStreamResult(await _copeService.CreateRandomCope(seed), "image/gif");
            span?.Finish(SpanStatus.Ok);
            return result;
        }

        [HttpGet("/{seed}/dl")]
        public async Task<IActionResult> DownloadCope(int seed)
        {
            var span = _sentryHub.GetSpan()?.StartChild("seeded download");
            Response.Headers.Add("Content-Disposition", $"attachment; filename=cope_{seed}.gif");
            var result = await CopeSeeded(seed);
            span?.Finish(SpanStatus.Ok);
            return result;
        }

    }
}
