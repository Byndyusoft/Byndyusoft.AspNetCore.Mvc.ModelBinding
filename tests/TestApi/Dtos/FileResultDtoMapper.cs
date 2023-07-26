using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Http;

namespace Byndyusoft.TestApi.Dtos
{
    public static class FileResultDtoMapper
    {
        public static FileResultDto MapFrom(IFormStreamedFile formStreamedFile, string filePath)
        {
            return new FileResultDto
            {
                Name = formStreamedFile.Name,
                FileName = formStreamedFile.FileName,
                ContentLength = formStreamedFile.ContentLength,
                FilePath = filePath
            };
        }

        public static FileResultDto MapFrom(IFormFile formFile, string filePath)
        {
            return new FileResultDto
            {
                Name = formFile.Name,
                FileName = formFile.FileName,
                ContentLength = formFile.Length,
                FilePath = filePath
            };
        }
    }
}