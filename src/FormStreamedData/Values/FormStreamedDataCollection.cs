using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values
{
    /// <summary>
    ///     Строковые данные из multipart form data и файлы, стримы которых еще не считаны
    /// </summary>
    public class FormStreamedDataCollection
    {
        public FormStreamedDataCollection(
            IDictionary<string, StringValues> fields,
            IAsyncEnumerable<IFormStreamedFile> files)
        {
            Fields = fields;
            Files = files;
        }

        /// <summary>
        ///     Список полей, которые не являются файлами
        /// </summary>
        public IDictionary<string, StringValues> Fields { get; }

        /// <summary>
        ///     Перечисление файлов с не считанными стримами
        /// </summary>
        public IAsyncEnumerable<IFormStreamedFile> Files { get; }
    }
}