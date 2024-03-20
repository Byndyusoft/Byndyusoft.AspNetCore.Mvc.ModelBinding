namespace Byndyusoft.TestApi.Dtos
{
    public class SaveResultWithSingleFileDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public FileResultDto File { get; set; } = default!;
    }
}