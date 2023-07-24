using System.Collections.Generic;
using System.Threading;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    public class StreamFormFileCollection : IAsyncEnumerable<IFormStreamedFile>
    {
        private readonly IAsyncEnumerable<IFormStreamedFile> _files;

        public StreamFormFileCollection(IAsyncEnumerable<IFormStreamedFile> files)
        {
            _files = files;
        }

        public IAsyncEnumerator<IFormStreamedFile> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _files.GetAsyncEnumerator(cancellationToken);
        }
    }
}