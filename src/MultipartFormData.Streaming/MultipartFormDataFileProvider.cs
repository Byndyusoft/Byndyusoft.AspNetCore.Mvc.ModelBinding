using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    public sealed class MultipartFormDataFileProvider : IMultipartFormDataFileProvider
    {
        public async Task<MultipartFormDataFileDto> GetAsync(HttpRequest request,
            CancellationToken cancellationToken)
        {
            EnsureRequestIsMultipartFormData(request);

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            var contentDisposition = GetContentDisposition(section);

            if (contentDisposition.IsFileDisposition() == false)
                throw new InvalidOperationException("Ожидалась только секция с файлом");

            var multipartFormDataFileDto = GetMultipartFormDataFileDto(section, contentDisposition);

            section = await reader.ReadNextSectionAsync(cancellationToken);
            if (section is not null)
                throw new InvalidOperationException("Ожидалась только одна секция формы");

            return multipartFormDataFileDto;
        }

        public async Task<IAsyncEnumerable<MultipartFormDataFileDto>> EnumerateAsync(
            HttpRequest request,
            CancellationToken cancellationToken)
        {
            EnsureRequestIsMultipartFormData(request);

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            return EnumerateFilesAsync(reader, section, cancellationToken);
        }

        public async Task<MultipartFormDataDto> GetFormDataAsync(
            HttpRequest request,
            CancellationToken cancellationToken)
        {
            EnsureRequestIsMultipartFormData(request);

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            var formAccumulator = new KeyValueAccumulator();
            while (section is not null)
            {
                var contentDisposition = GetContentDisposition(section);

                if (contentDisposition.IsFormDisposition() == false)
                    break;

                var formDataSection = new FormMultipartSection(section, contentDisposition);

                var key = formDataSection.Name;
                var value = await formDataSection.GetValueAsync();
                formAccumulator.Append(key, value);

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            var files = EnumerateFilesAsync(reader, section, cancellationToken);
            var multipartFormDataDto = new MultipartFormDataDto(formAccumulator.GetResults(), files);

            return multipartFormDataDto;
        }

        private static ContentDispositionHeaderValue GetContentDisposition(MultipartSection section)
        {
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) == false)
                throw new InvalidOperationException("Header Content-Disposition не найден");
            return contentDisposition;
        }

        private static void EnsureRequestIsMultipartFormData(HttpRequest request)
        {
            MediaTypeHeaderValue.TryParse(request.ContentType, out var contentType);
            if (HasMultipartFormContentType(contentType) == false)
                throw new InvalidOperationException("Content type is not multipart form data");
        }

        private async IAsyncEnumerable<MultipartFormDataFileDto> EnumerateFilesAsync(
            MultipartReader multipartReader,
            MultipartSection? currentSection,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var section = currentSection;

            while (section is not null)
            {
                var contentDisposition = GetContentDisposition(section);

                if (contentDisposition.IsFileDisposition() == false)
                    throw new InvalidOperationException("Ожидались только секции с файлами");

                var multipartFormDataFileDto = GetMultipartFormDataFileDto(section, contentDisposition);
                yield return multipartFormDataFileDto;

                section = await multipartReader.ReadNextSectionAsync(cancellationToken);
            }
        }

        private MultipartFormDataFileDto GetMultipartFormDataFileDto(
            MultipartSection section,
            ContentDispositionHeaderValue contentDisposition)
        {
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