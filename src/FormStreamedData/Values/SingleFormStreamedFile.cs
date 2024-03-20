using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values
{
    /// <summary>
    ///     Обертка над асинхронным перечислением файлов, потоки которых предназначены для считывания пользователем.
    ///     Файлы не считываются во время биндинга.
    ///     Свойство этого типа должно быть единственным в запросе.
    /// </summary>
    public class SingleFormStreamedFile
    {
        private readonly IAsyncEnumerable<IFormStreamedFile> _files;
        private IFormStreamedFile? _file;

        public SingleFormStreamedFile(IAsyncEnumerable<IFormStreamedFile> files)
        {
            _files = files;
        }

        public async Task<IFormStreamedFile> ReadFileAsync(CancellationToken cancellationToken = default)
        {
            if (_file is null)
            {
                var asyncEnumerator = _files.GetAsyncEnumerator(cancellationToken);
                await asyncEnumerator.MoveNextAsync();
                _file = asyncEnumerator.Current;
            }

            return _file;
        }
    }
}