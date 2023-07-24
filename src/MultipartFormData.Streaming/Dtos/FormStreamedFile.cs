using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos
{
    public class FormStreamedFile : IFormStreamedFile
    {
        // Stream.CopyTo method uses 80KB as the default buffer size.
        private const int DefaultBufferSize = 80 * 1024;

        private readonly Stream _stream;

        public FormStreamedFile(Stream stream, string name, string fileName, IHeaderDictionary headers)
        {
            _stream = stream;
            Name = name;
            FileName = fileName;
            Headers = headers;
        }

        /// <summary>
        /// Gets the raw Content-Disposition header of the uploaded file.
        /// </summary>
        public string ContentDisposition => Headers[HeaderNames.ContentDisposition];

        /// <summary>
        /// Gets the Content-Length header value of the uploaded file.
        /// </summary>
        public long? ContentLength
        {
            get
            {
                var contentLengthHeader = (string)Headers[HeaderNames.ContentLength];
                if (string.IsNullOrEmpty(contentLengthHeader) == false 
                    && long.TryParse(contentLengthHeader, out var contentLength))
                    return contentLength;

                return null;
            }
        }

        /// <summary>
        /// Gets the raw Content-Type header of the uploaded file.
        /// </summary>
        public string ContentType => Headers[HeaderNames.ContentType];

        /// <summary>
        /// Gets the header dictionary of the uploaded file.
        /// </summary>
        public IHeaderDictionary Headers { get; }

        /// <summary>
        /// Gets the file length in bytes.
        /// </summary>
        public long Length => throw new NotImplementedException("Length is not known because of stream");

        /// <summary>
        /// Gets the name from the Content-Disposition header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the file name from the Content-Disposition header.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Opens the request stream for reading the uploaded file.
        /// </summary>
        public Stream OpenReadStream()
        {
            return _stream;
        }

        /// <summary>
        /// Copies the contents of the uploaded file to the <paramref name="target"/> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        public void CopyTo(Stream target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using var readStream = OpenReadStream();
            readStream.CopyTo(target, DefaultBufferSize);
        }

        /// <summary>
        /// Asynchronously copies the contents of the uploaded file to the <paramref name="target"/> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        /// <param name="cancellationToken"></param>
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            await using var readStream = OpenReadStream();
            await readStream.CopyToAsync(target, DefaultBufferSize, cancellationToken);
        }
    }
}