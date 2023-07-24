using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Attributes;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders;
using Byndyusoft.Example.Dtos;
using Byndyusoft.Example.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Example.Controllers
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
        public async Task<ActionResult<SaveResultDto>> SaveOldWay([FromForm] OldRequestDto requestDto, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();

            foreach (var file in requestDto.Files)
            {
                await using var stream = file.OpenReadStream();
                var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
                resultDtos.Add(FileResultDtoMapper.MapFrom(file, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveOldWay), stopwatch.Elapsed);

            return SaveResultDtoMapper.MapFrom(requestDto, resultDtos.ToArray());
        }

        [HttpPost("SaveNew")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueModelBinding]
        public async Task<ActionResult<SaveResultDto>> SaveNewWay([FromFormData] NewRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();

            await foreach (var dto in requestDto.Files.WithCancellation(cancellationToken))
            {
                var filePath = await _fileService.SaveFileAsync(dto.Stream, dto.FileName, cancellationToken);
                resultDtos.Add(FileResultDtoMapper.MapFrom(dto, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return SaveResultDtoMapper.MapFrom(requestDto, resultDtos.ToArray());
        }
    }
}