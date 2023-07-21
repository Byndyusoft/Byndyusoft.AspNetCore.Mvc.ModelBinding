using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming;
using Microsoft.AspNetCore.Http;

namespace Byndyusoft.Example.Dtos
{
    public static class ResultDtoMapper
    {
        public static ResultDto MapFrom(MultipartFormDataFileDto multipartFormDataFileDto, string filePath)
        {
            return new ResultDto
            {
                Name = multipartFormDataFileDto.Name,
                FileName = multipartFormDataFileDto.FileName,
                ContentLength = multipartFormDataFileDto.ContentLength,
                FilePath = filePath
            };
        }

        public static ResultDto MapFrom(IFormFile formFile, string filePath)
        {
            return new ResultDto
            {
                Name = formFile.Name,
                FileName = formFile.FileName,
                ContentLength = formFile.Length,
                FilePath = filePath
            };
        }
    }
}