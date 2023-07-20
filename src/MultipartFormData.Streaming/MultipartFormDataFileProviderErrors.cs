using Byndyusoft.ModelResult.ModelResults;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    /// <summary>
    ///     Ошибки извлечения файла из multipart form data
    /// </summary>
    public static class MultipartFormDataFileProviderErrors
    {
        public static string Code => "MultipartFormDataFileProvider.Error";

        public static ErrorModelResult FromMessage(string message)
        {
            return new ErrorModelResult(Code, message);
        }
    }
}