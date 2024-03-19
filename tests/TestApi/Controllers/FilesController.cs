using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Attributes;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Byndyusoft.TestApi.Dtos;
using Byndyusoft.TestApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.TestApi.Controllers
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
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<SaveResultDto>> SaveNewWay(
            [FromFormStreamedData] NewRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();

            await foreach (var file in requestDto.StreamedFiles.WithCancellation(cancellationToken))
            {
                await using var stream = file.OpenReadStream();
                var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
                resultDtos.Add(FileResultDtoMapper.MapFrom(file, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return SaveResultDtoMapper.MapFrom(requestDto, resultDtos.ToArray());
        }

        [HttpPost("SaveNewByParameter")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<FileResultDto[]>> SaveNewWayByParameter(
            FormStreamedFileCollection files,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();

            await foreach (var file in files.WithCancellation(cancellationToken))
            {
                await using var stream = file.OpenReadStream();
                var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
                resultDtos.Add(FileResultDtoMapper.MapFrom(file, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return resultDtos.ToArray();
        }

        [HttpPost("SaveNewBySingleFile")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<SaveResultWithSingleFileDto>> SaveNewBySingleFile(
            [FromFormStreamedData] NewRequestWithSingleFileDto requestDto,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var file = await requestDto.StreamedFile.ReadFileAsync(cancellationToken);
            await using var stream = file.OpenReadStream();
            var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
            var fileResultDto = FileResultDtoMapper.MapFrom(file, filePath);

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return SaveResultDtoMapper.MapFrom(requestDto, fileResultDto);
        }

        [HttpPost("SaveNewIncorrectly")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<FileResultDto[]>> SaveNewWayIncorrectly(
            FormStreamedFileCollection files,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var formStreamedFiles = new List<IFormStreamedFile>();
            await foreach (var file in files.WithCancellation(cancellationToken)) 
                formStreamedFiles.Add(file);

            var resultDtos = new List<FileResultDto>();
            foreach (var file in formStreamedFiles)
            {
                await using var stream = file.OpenReadStream();
                var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
                resultDtos.Add(FileResultDtoMapper.MapFrom(file, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return resultDtos.ToArray();
        }

        [HttpPost("HashOld")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<string[]>> HashOldWay([FromForm] OldRequestDto requestDto, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var hashes = new List<string>();

            foreach (var file in requestDto.Files)
            {
                await using var stream = file.OpenReadStream();
                var hash = await _fileService.CalculateHashAsync(stream, cancellationToken);
                hashes.Add(hash);
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(HashOldWay), stopwatch.Elapsed);

            return hashes.ToArray();
        }

        [HttpPost("HashNew")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<string[]>> HashNewWay(
            [FromFormStreamedData] NewRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var hashes = new List<string>();

            await foreach (var file in requestDto.StreamedFiles.WithCancellation(cancellationToken))
            {
                await using var stream = file.OpenReadStream();
                var hash = await _fileService.CalculateHashAsync(stream, cancellationToken);
                hashes.Add(hash);
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(HashNewWay), stopwatch.Elapsed);

            return hashes.ToArray();
        }

        [HttpPost("UploadOld")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<string[]>> UploadOldWay([FromForm] OldRequestDto requestDto, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var objectNames = new List<string>();

            foreach (var file in requestDto.Files)
            {
                await using var stream = file.OpenReadStream();
                var objectName = await _fileService.UploadToStorageAsync(stream, file.FileName, file.Length, cancellationToken);
                objectNames.Add(objectName);
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(UploadOldWay), stopwatch.Elapsed);

            return objectNames.ToArray();
        }

        [HttpPost("UploadNew")]
        [RequestSizeLimit(long.MaxValue)]
        [SetFormStreamedDataValueProvider]
        public async Task<ActionResult<string[]>> UploadNewWay(
            [FromFormStreamedData] NewRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var objectNames = new List<string>();

            await foreach (var file in requestDto.StreamedFiles.WithCancellation(cancellationToken))
            {
                if (file.ContentLength is null)
                    return BadRequest("Content Length is required");

                await using var stream = file.OpenReadStream();
                var objectName = await _fileService.UploadToStorageAsync(
                    stream, 
                    file.FileName,
                    file.ContentLength.Value, 
                    cancellationToken);
                objectNames.Add(objectName);
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(UploadNewWay), stopwatch.Elapsed);

            return objectNames.ToArray();
        }
    }
}