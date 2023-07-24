using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for <see cref="IFormFile"/>, collections
    /// of <see cref="IFormFile"/>, and <see cref="IFormFileCollection"/>.
    /// </summary>
    public class FormDataFileModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            // Note: This condition needs to be kept in sync with ApiBehaviorApplicationModelProvider.
            var modelType = context.Metadata.ModelType;
            if (modelType == typeof(StreamFormFileCollection))
                return new FormDataFileModelBinder();

            return null;
        }
    }
}