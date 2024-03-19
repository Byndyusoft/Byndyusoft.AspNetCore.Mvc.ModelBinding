using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;

namespace Byndyusoft.TestApi.Dtos
{
    public class NewRequestWithSingleFileDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public SingleFormStreamedFile StreamedFile { get; set; } = default!;
    }
}