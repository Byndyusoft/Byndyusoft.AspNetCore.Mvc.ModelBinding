using System.IO;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    /// <summary>
    ///     Данные файла из multipart form data
    /// </summary>
    public class MultipartFormDataFileDto
    {
        /// <summary>
        ///     Имя файла
        /// </summary>
        public string FileName { get; set; } = default!;

        /// <summary>
        ///     Длина файла
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        ///     Поток чтения файла
        /// </summary>
        public Stream Stream { get; set; } = default!;
    }
}