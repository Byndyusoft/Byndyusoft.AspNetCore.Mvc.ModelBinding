using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.ModelResult.ModelResults;
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
        /// <param name="request">HTTP Request c файлом в multipart from data</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>
        ///     Возвращает имя, длину файла и поток для чтения
        ///     Возвращает ошибку, если извлечь файл не получилось 
        /// </returns>
        Task<ModelResult<MultipartFormDataFileDto>> GetAsync(HttpRequest request, CancellationToken cancellationToken);
    }
}