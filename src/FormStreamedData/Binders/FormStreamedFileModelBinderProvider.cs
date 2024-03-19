using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     Провайдер класса для связывания данных типа <see cref="FormStreamedFileCollection" />.
    /// </summary>
    public class FormStreamedFileModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;
            if (modelType == typeof(FormStreamedFileCollection) || modelType == typeof(SingleFormStreamedFile))
                return new FormStreamedFileModelBinder();

            return null;
        }
    }
}