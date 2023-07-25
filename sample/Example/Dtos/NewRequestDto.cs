using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Values;

namespace Byndyusoft.Example.Dtos
{
    public class NewRequestDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public FormStreamedFileCollection StreamedFiles { get; set; } = default!;
    }
}