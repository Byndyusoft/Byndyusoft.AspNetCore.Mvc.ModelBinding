using System.Collections.Generic;
using System.Threading;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Values
{
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