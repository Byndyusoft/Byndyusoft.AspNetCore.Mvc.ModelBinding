﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public static class FormStreamedDataExtensions
    {
        public static async Task<FormStreamedDataCollection> ReadFormStreamedDataAsync(
            this HttpRequest request,
            CancellationToken cancellationToken = default)
        {
            var httpContextKey = "Byndyusoft.FormData.Stream";
            if (request.HttpContext.Items.TryGetValue(httpContextKey, out var dtoObject) && dtoObject is FormStreamedDataCollection multipartFormDataDto)
                return multipartFormDataDto;

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
            multipartFormDataDto = new FormStreamedDataCollection(formAccumulator.GetResults(), files);

            request.HttpContext.Items[httpContextKey] = multipartFormDataDto;

            return multipartFormDataDto;
        }

        private static void EnsureRequestIsMultipartFormData(HttpRequest request)
        {
            MediaTypeHeaderValue.TryParse(request.ContentType, out var contentType);
            if (HasMultipartFormContentType(contentType) == false)
                throw new InvalidOperationException("Content type is not multipart form data");
        }

        private static bool HasMultipartFormContentType([NotNullWhen(true)] MediaTypeHeaderValue? contentType)
        {
            // Content-Type: multipart/form-data; boundary=----WebKitFormBoundarymx2fSWqWSd0OxQqq
            return contentType != null && contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
        }

        private static ContentDispositionHeaderValue GetContentDisposition(MultipartSection section)
        {
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) == false)
                throw new InvalidOperationException("Header Content-Disposition не найден");
            return contentDisposition;
        }

        private static async IAsyncEnumerable<MultipartFormDataFileDto> EnumerateFilesAsync(
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

        private static MultipartFormDataFileDto GetMultipartFormDataFileDto(
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

        private static long? GetContentLength(MultipartSection multipartSection)
        {
            var contentLengthHeader = GetHeaderSingleValue(multipartSection, "Content-Length");
            if (string.IsNullOrEmpty(contentLengthHeader))
                return null;

            if (long.TryParse(contentLengthHeader, out var contentLength) == false)
                return null;

            return contentLength;
        }

        private static string? GetHeaderSingleValue(MultipartSection section, string headerName)
        {
            if (section.Headers.TryGetValue(headerName, out var headerStringValues) == false)
                return null;

            return headerStringValues.SingleOrDefault();
        }
    }
}