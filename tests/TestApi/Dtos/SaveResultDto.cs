using System;

namespace Byndyusoft.TestApi.Dtos
{
    public class SaveResultDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public FileResultDto[] Files { get; set; } = Array.Empty<FileResultDto>();
    }
}