using DotCope.Coping;
using Microsoft.AspNetCore.Mvc;

namespace DotCope
{
    [Route("/")]
    [ApiController]
    public class CopeController : ControllerBase
    {
        private readonly CopeService _copeService;

        public CopeController(CopeService copeService)
        {
            _copeService = copeService;
        }

        [HttpGet]
        public async Task<IActionResult> Cope()
        {
            Stream outStream = await _copeService.CreateRandomCope();
            return new FileStreamResult(outStream, "image/gif");
        }

    }
}
