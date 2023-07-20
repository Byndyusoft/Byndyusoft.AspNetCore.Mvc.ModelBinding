using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Example.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly FileService _fileService;

        public FilesController(
            ILogger<FilesController> logger,
            FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [HttpPost("SaveOld")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> SaveOldWay([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            await using var stream = file.OpenReadStream();
            await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveOldWay), stopwatch.Elapsed);

            return Ok();
        }
    }
}