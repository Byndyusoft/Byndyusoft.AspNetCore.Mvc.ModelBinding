using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for <see cref="IFormFile"/>, collections
    /// of <see cref="IFormFile"/>, and <see cref="IFormFileCollection"/>.
    /// </summary>
    public class FormStreamedFileModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;
            if (modelType == typeof(FormStreamedFileCollection))
                return new FormStreamedFileModelBinder();

            return null;
        }
    }
}