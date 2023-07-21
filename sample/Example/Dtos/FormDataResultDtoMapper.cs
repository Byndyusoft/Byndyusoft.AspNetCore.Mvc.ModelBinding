using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;

namespace Byndyusoft.Example.Dtos
{
    public static class FormDataResultDtoMapper
    {
        public static FormDataResultDto MapFrom(MultipartFormDataDto multipartFormDataDto, FileResultDto[] fileResultDtos)
        {
            return new FormDataResultDto
            {
                Fields = multipartFormDataDto.Fields,
                Files = fileResultDtos
            };
        }
    }
}