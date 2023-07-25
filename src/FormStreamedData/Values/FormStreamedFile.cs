using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values
{
    /// <summary>
    ///     Информация о файле из form-data, стримы которых считываются пользователем после биндинга.
    /// </summary>
    public class FormStreamedFile : IFormStreamedFile
    {
        // Метод Stream.CopyTo использует 80KB в качестве размера буфера по умолчанию.
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
        ///     Возвращает заголовок Content-Disposition секции файла.
        /// </summary>
        public string ContentDisposition => Headers[HeaderNames.ContentDisposition];

        /// <summary>
        ///     Возвращает численное значение Content-Length секции файла, если есть
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
        ///     Возвращает заголовок Content-Type секции файла.
        /// </summary>
        public string ContentType => Headers[HeaderNames.ContentType];

        /// <summary>
        ///     Словарь заголовков секции файла.
        /// </summary>
        public IHeaderDictionary Headers { get; }

        /// <summary>
        ///     Не реализовано в этой реализации, т.к. длина файла неизвестна.
        /// </summary>
        public long Length => throw new NotImplementedException("Length is not known because of stream");

        /// <summary>
        ///     Возвращает имя из заголовка Content-Disposition секции файла.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Возвращает имя файла из заголовка Content-Disposition секции файла.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        ///     Возвращает стрим файла, который можно считать только один раз.
        /// </summary>
        public Stream OpenReadStream()
        {
            return _stream;
        }

        /// <summary>
        ///     Копирует содержимое файла в стрим <paramref name="target" />.
        /// </summary>
        /// <param name="target">Стрим, в который нужно скопировать содержимое файла.</param>
        public void CopyTo(Stream target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            using var readStream = OpenReadStream();
            readStream.CopyTo(target, DefaultBufferSize);
        }

        /// <summary>
        ///     Асинхронно копирует содержимое файла в стрим <paramref name="target" />.
        /// </summary>
        /// <param name="target">Стрим, в который нужно скопировать содержимое файла.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            await using var readStream = OpenReadStream();
            await readStream.CopyToAsync(target, DefaultBufferSize, cancellationToken);
        }
    }
}