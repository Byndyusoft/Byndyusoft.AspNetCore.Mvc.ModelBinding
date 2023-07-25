using Microsoft.AspNetCore.Http;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values
{
    public interface IFormStreamedFile : IFormFile
    {
        /// <summary>
        ///     Возвращает численное значение Content-Length секции файла, если есть
        /// </summary>
        long? ContentLength { get; }
    }
}