﻿using Microsoft.AspNetCore.Http;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos
{
    public interface IFormStreamedFile : IFormFile
    {
        /// <summary>
        /// Gets the Content-Length header value of the uploaded file.
        /// </summary>
        long? ContentLength { get; }
    }
}