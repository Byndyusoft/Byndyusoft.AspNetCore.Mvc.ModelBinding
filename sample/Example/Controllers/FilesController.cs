using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Attributes;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Byndyusoft.Example.Services;
using Byndyusoft.ModelResult.AspNetCore.Extensions;
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
        private readonly IMultipartFormDataFileProvider _multipartFormDataFileProvider;

        public FilesController(
            ILogger<FilesController> logger,
            FileService fileService,
            IMultipartFormDataFileProvider multipartFormDataFileProvider)
        {
            _logger = logger;
            _fileService = fileService;
            _multipartFormDataFileProvider = multipartFormDataFileProvider;
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

        [HttpPost("SaveNew")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult> SaveNewWay(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var fileDtoResult = await _multipartFormDataFileProvider.GetAsync(Request, cancellationToken);
            if (fileDtoResult.IsError())
                return fileDtoResult.AsSimple().ToActionResult();

            var multipartFormDataFileDto = fileDtoResult.Result;
            await _fileService.SaveFileAsync(multipartFormDataFileDto.Stream, multipartFormDataFileDto.FileName, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveOldWay), stopwatch.Elapsed);

            return Ok();
        }
    }
}