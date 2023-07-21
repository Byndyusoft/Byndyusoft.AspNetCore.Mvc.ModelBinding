using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Attributes;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Byndyusoft.Example.Dtos;
using Byndyusoft.Example.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<FileResultDto>> SaveOldWay([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            await using var stream = file.OpenReadStream();
            var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveOldWay), stopwatch.Elapsed);

            return ResultDtoMapper.MapFrom(file, filePath);
        }

        [HttpPost("SaveNew")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<FileResultDto>> SaveNewWay(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var dto = await _multipartFormDataFileProvider.GetAsync(Request, cancellationToken);
            var filePath = await _fileService.SaveFileAsync(dto.Stream, dto.FileName, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewWay), stopwatch.Elapsed);

            return ResultDtoMapper.MapFrom(dto, filePath);
        }

        [HttpPost("SaveNewMultiple")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<FileResultDto[]>> SaveNewMultiple(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();
            var fileDtos = await _multipartFormDataFileProvider.EnumerateAsync(Request, cancellationToken);

            await foreach (var dto in fileDtos.WithCancellation(cancellationToken))
            {
                var filePath = await _fileService.SaveFileAsync(dto.Stream, dto.FileName, cancellationToken);
                resultDtos.Add(ResultDtoMapper.MapFrom(dto, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveNewMultiple), stopwatch.Elapsed);

            return resultDtos.ToArray();
        }

        [HttpPost("SaveFormData")]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<FormDataResultDto>> SaveFormData(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var resultDtos = new List<FileResultDto>();
            var formDataDto = await _multipartFormDataFileProvider.GetFormDataAsync(Request, cancellationToken);

            await foreach (var dto in formDataDto.Files.WithCancellation(cancellationToken))
            {
                var filePath = await _fileService.SaveFileAsync(dto.Stream, dto.FileName, cancellationToken);
                resultDtos.Add(ResultDtoMapper.MapFrom(dto, filePath));
            }

            stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {Elapsed}", nameof(SaveFormData), stopwatch.Elapsed);

            return FormDataResultDtoMapper.MapFrom(formDataDto, resultDtos.ToArray());
        }
    }
}