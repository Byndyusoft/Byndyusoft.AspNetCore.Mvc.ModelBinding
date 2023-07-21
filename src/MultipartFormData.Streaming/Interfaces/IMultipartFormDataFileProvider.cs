using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;
using Microsoft.AspNetCore.Http;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces
{
    /// <summary>
    ///     Служба извлечения файла из multipart form data
    /// </summary>
    public interface IMultipartFormDataFileProvider
    {
        /// <summary>
        ///     Извлекает метаданные файла и поток для чтения
        /// </summary>
        /// <param name="request">HTTP Request c файлом в multipart form data</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        ///     Возвращает имя, длину файла и поток для чтения
        ///     Возвращает ошибку, если извлечь файл не получилось
        /// </returns>
        Task<MultipartFormDataFileDto> GetAsync(HttpRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Извлекает метаданные файлов и потоки для чтения
        /// </summary>
        /// <param name="request">HTTP Request с файлами в multipart form data</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        ///     Перечисление файлов, стримы которых нужно считывать по порядку
        /// </returns>
        Task<IAsyncEnumerable<MultipartFormDataFileDto>> EnumerateAsync(
            HttpRequest request,
            CancellationToken cancellationToken);

        Task<Dtos.MultipartFormDataCollection> GetFormDataAsync(
            HttpRequest request,
            CancellationToken cancellationToken);
    }
}