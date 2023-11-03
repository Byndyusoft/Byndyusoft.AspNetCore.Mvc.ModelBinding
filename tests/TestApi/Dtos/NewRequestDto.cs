using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;

namespace Byndyusoft.TestApi.Dtos
{
    public class NewRequestDto
    {
        public string Name { get; set; } = default!;

        public int Age { get; set; }

        public FormStreamedFileCollection StreamedFiles { get; set; } = default!;
    }
}