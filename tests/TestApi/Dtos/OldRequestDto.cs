using Microsoft.AspNetCore.Http;

namespace Byndyusoft.TestApi.Dtos
{
    public class OldRequestDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public IFormFileCollection Files { get; set; } = default!;
    }
}