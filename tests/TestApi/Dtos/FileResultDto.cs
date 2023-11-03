namespace Byndyusoft.TestApi.Dtos
{
    public class FileResultDto
    {
        public string Name { get; set; } = default!;

        public string FileName { get; set; } = default!;

        public long? ContentLength { get; set; }

        public string FilePath { get; set; } = default!;
    }
}