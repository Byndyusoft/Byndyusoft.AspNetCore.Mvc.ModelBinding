using System.Collections.Generic;
using System.Threading;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    public class StreamFormFileCollection : IAsyncEnumerable<MultipartFormDataFileDto>
    {
        private readonly IAsyncEnumerable<MultipartFormDataFileDto> _files;

        public StreamFormFileCollection(IAsyncEnumerable<MultipartFormDataFileDto> files)
        {
            _files = files;
        }

        public IAsyncEnumerator<MultipartFormDataFileDto> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _files.GetAsyncEnumerator(cancellationToken);
        }
    }
}