using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders;

namespace Byndyusoft.Example.Dtos
{
    public class NewRequestDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public StreamFormFileCollection Files { get; set; } = default!;
    }
}