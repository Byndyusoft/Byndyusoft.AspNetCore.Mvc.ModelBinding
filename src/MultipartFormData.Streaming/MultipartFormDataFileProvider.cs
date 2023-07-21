using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Byndyusoft.ModelResult.ModelResults;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    public sealed class MultipartFormDataFileProvider : IMultipartFormDataFileProvider
    {
        public async Task<ModelResult<MultipartFormDataFileDto>> GetAsync(HttpRequest request, CancellationToken cancellationToken)
        {
            MediaTypeHeaderValue.TryParse(request.ContentType, out var contentType);
            if (HasMultipartFormContentType(contentType) == false)
                return MultipartFormDataFileProviderErrors.FromMessage("Content type is not multipart form data");

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            while (section is not null)
            {
                var multipartFormDataFileDtoResult = GetMultipartFormDataFileDto(section);
                if (multipartFormDataFileDtoResult.IsError())
                    return multipartFormDataFileDtoResult.AsSimple();

                var multipartFormDataFileDto = multipartFormDataFileDtoResult.Result;
                if (multipartFormDataFileDto is not null)
                    return multipartFormDataFileDto;

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            return MultipartFormDataFileProviderErrors.FromMessage("Контент файла не найден");
        }

        public IAsyncEnumerable<MultipartFormDataFileDto> EnumerateAsync(
            HttpRequest request,
            CancellationToken cancellationToken)
        {
            MediaTypeHeaderValue.TryParse(request.ContentType, out var contentType);
            if (HasMultipartFormContentType(contentType) == false)
                throw new InvalidOperationException("Content type is not multipart form data");

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            return EnumerateAsync(reader, cancellationToken);
        }

        private async IAsyncEnumerable<MultipartFormDataFileDto> EnumerateAsync(
            MultipartReader multipartReader,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var section = await multipartReader.ReadNextSectionAsync(cancellationToken);

            while (section is not null)
            {
                var multipartFormDataFileDtoResult = GetMultipartFormDataFileDto(section);
                if (multipartFormDataFileDtoResult.IsError())
                    throw new InvalidOperationException(multipartFormDataFileDtoResult.AsError().Message);

                var multipartFormDataFileDto = multipartFormDataFileDtoResult.Result;
                if (multipartFormDataFileDto is not null)
                    yield return multipartFormDataFileDto;

                section = await multipartReader.ReadNextSectionAsync(cancellationToken);
            }
        }

        private ModelResult<MultipartFormDataFileDto?> GetMultipartFormDataFileDto(MultipartSection section)
        {
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) == false)
                return MultipartFormDataFileProviderErrors.FromMessage("Header Content-Disposition не найден");

            if (contentDisposition.IsFileDisposition() == false)
                return (MultipartFormDataFileDto?)null;

            var fileSection = new FileMultipartSection(section, contentDisposition);

            var name = fileSection.Name;
            var fileName = fileSection.FileName;

            var contentLength = GetContentLength(section);

            return new MultipartFormDataFileDto
            {
                Stream = fileSection.FileStream,
                Name = name,
                FileName = fileName,
                ContentLength = contentLength
            };
        }

        private long? GetContentLength(MultipartSection multipartSection)
        {
            var contentLengthHeader = GetHeaderSingleValue(multipartSection, "Content-Length");
            if (string.IsNullOrEmpty(contentLengthHeader))
                return null;

            if (long.TryParse(contentLengthHeader, out var contentLength) == false)
                return null;

            return contentLength;
        }

        private string? GetHeaderSingleValue(MultipartSection section, string headerName)
        {
            if (section.Headers.TryGetValue(headerName, out var headerStringValues) == false)
                return null;

            return headerStringValues.SingleOrDefault();
        }

        private static bool HasMultipartFormContentType([NotNullWhen(true)] MediaTypeHeaderValue? contentType)
        {
            // Content-Type: multipart/form-data; boundary=----WebKitFormBoundarymx2fSWqWSd0OxQqq
            return contentType != null && contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
        }
    }
}