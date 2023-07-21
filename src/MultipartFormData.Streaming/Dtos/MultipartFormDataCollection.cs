using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos
{
    /// <summary>
    ///     Данные из multipart form data
    /// </summary>
    public class MultipartFormDataCollection
    {
        public MultipartFormDataCollection(Dictionary<string, StringValues> fields,
            IAsyncEnumerable<MultipartFormDataFileDto> files)
        {
            Fields = fields;
            Files = files;
        }

        /// <summary>
        ///     Список полей, которые не являются файлами
        /// </summary>
        public Dictionary<string, StringValues> Fields { get; }

        /// <summary>
        ///     Перечисление файлов со стримами
        /// </summary>
        public IAsyncEnumerable<MultipartFormDataFileDto> Files { get; }
    }
}