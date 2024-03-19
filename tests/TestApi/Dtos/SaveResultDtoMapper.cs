namespace Byndyusoft.TestApi.Dtos
{
    public static class SaveResultDtoMapper
    {
        public static SaveResultDto MapFrom(NewRequestDto newRequestDto, FileResultDto[] fileResultDtos)
        {
            return new SaveResultDto
            {
                Name = newRequestDto.Name,
                Age = newRequestDto.Age,
                Files = fileResultDtos
            };
        }

        public static SaveResultDto MapFrom(OldRequestDto oldRequestDto, FileResultDto[] fileResultDtos)
        {
            return new SaveResultDto
            {
                Name = oldRequestDto.Name,
                Age = oldRequestDto.Age,
                Files = fileResultDtos
            };
        }

        public static SaveResultWithSingleFileDto MapFrom(NewRequestWithSingleFileDto newRequestDto, FileResultDto fileResultDto)
        {
            return new SaveResultWithSingleFileDto
            {
                Name = newRequestDto.Name,
                Age = newRequestDto.Age,
                File = fileResultDto
            };
        }
    }
}