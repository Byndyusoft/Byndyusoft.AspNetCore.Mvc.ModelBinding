using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;
using Microsoft.AspNetCore.Http;

namespace Byndyusoft.Example.Dtos
{
    public static class ResultDtoMapper
    {
        public static FileResultDto MapFrom(MultipartFormDataFileDto multipartFormDataFileDto, string filePath)
        {
            return new FileResultDto
            {
                Name = multipartFormDataFileDto.Name,
                FileName = multipartFormDataFileDto.FileName,
                ContentLength = multipartFormDataFileDto.ContentLength,
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