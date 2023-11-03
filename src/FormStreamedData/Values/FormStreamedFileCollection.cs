using System.Collections.Generic;
using System.Threading;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values
{
    /// <summary>
    ///     Обертка над асинхронным перечислением файлов, потоки которых предназначены для считывания пользователем.
    ///     Файлы не считываются во время биндинга.
    ///     Потоки можно считать только один раз.
    /// </summary>
    public class FormStreamedFileCollection : IAsyncEnumerable<IFormStreamedFile>
    {
        private readonly IAsyncEnumerable<IFormStreamedFile> _files;

        public FormStreamedFileCollection(IAsyncEnumerable<IFormStreamedFile> files)
        {
            _files = files;
        }

        public IAsyncEnumerator<IFormStreamedFile> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _files.GetAsyncEnumerator(cancellationToken);
        }
    }
}