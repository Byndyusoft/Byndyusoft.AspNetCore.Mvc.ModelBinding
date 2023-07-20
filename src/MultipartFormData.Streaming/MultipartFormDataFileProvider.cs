using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Byndyusoft.ModelResult.ModelResults;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    public sealed class MultipartFormDataFileProvider : IMultipartFormDataFileProvider
    {
        public async Task<ModelResult<MultipartFormDataFileDto>> GetAsync(HttpRequest request, CancellationToken cancellationToken)
        {
            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);
            if (section == null)
                return MultipartFormDataFileProviderErrors.FromMessage("Контент файла не найден");

            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) == false)
                return MultipartFormDataFileProviderErrors.FromMessage("Header Content-Disposition не найден");

            if (contentDisposition.DispositionType.Equals("form-data") == false)
                return MultipartFormDataFileProviderErrors.FromMessage("Header Content-Disposition имеет некорректный disposition type");

            var fileName = contentDisposition.FileNameStar ?? contentDisposition.FileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(fileName);

            if (string.IsNullOrEmpty(fileName))
                return MultipartFormDataFileProviderErrors.FromMessage("Header Content-Disposition не содержит имя файла");

            var contentLengthResult = GetContentLength(section);
            if (contentLengthResult.IsError())
                return contentLengthResult.AsSimple();

            return new MultipartFormDataFileDto { FileName = trustedFileNameForDisplay, ContentLength = contentLengthResult.Result, Stream = section.Body };
        }

        private ModelResult<long?> GetContentLength(MultipartSection multipartSection)
        {
            var contentLengthHeader = GetHeaderSingleValue(multipartSection, "Content-Length");
            if (string.IsNullOrEmpty(contentLengthHeader))
                return (long?)null;

            if (long.TryParse(contentLengthHeader, out var contentLength) == false)
                return MultipartFormDataFileProviderErrors.FromMessage("Header Content-Length имеет некорректный формат");

            return contentLength;
        }

        private string? GetHeaderSingleValue(MultipartSection section, string headerName)
        {
            if (section.Headers is null)
                return null;

            if (section.Headers.TryGetValue(headerName, out var headerStringValues) == false)
                return null;

            return headerStringValues.SingleOrDefault();
        }
    }
}