using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;

namespace Byndyusoft.Example.Dtos
{
    public static class FormDataResultDtoMapper
    {
        public static FormDataResultDto MapFrom(MultipartFormDataCollection multipartFormDataCollection, FileResultDto[] fileResultDtos)
        {
            return new FormDataResultDto
            {
                Fields = multipartFormDataCollection.Fields,
                Files = fileResultDtos
            };
        }
    }
}