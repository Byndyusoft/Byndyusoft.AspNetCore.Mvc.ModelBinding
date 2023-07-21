using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Byndyusoft.Example.Dtos
{
    public class FormDataResultDto
    {
        public Dictionary<string, StringValues> Fields { get; set; } = new();

        public FileResultDto[] Files { get; set; } = Array.Empty<FileResultDto>();
    }
}