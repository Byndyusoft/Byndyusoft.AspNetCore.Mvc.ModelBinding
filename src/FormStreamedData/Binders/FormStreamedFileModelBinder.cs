using System.Collections.Generic;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Extensions;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     Класс для связывания данных типа <see cref="FormStreamedFileCollection" />.
    ///     По сути возвращает перечисление для считывания стримов файлов.
    /// </summary>
    public class FormStreamedFileModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // If we're at the top level, then use the FieldName (parameter or property name).
            // This handles the fact that there will be nothing in the ValueProviders for this parameter
            // and so we'll do the right thing even though we 'fell-back' to the empty prefix.
            var modelName = bindingContext.IsTopLevelObject
                ? bindingContext.BinderModelName ?? bindingContext.FieldName
                : bindingContext.ModelName;

            var files = await GetFormFilesAsync(bindingContext);
            var value = new FormStreamedFileCollection(files);

            // We need to add a ValidationState entry because the modelName might be non-standard. Otherwise
            // the entry we create in model state might not be marked as valid.
            bindingContext.ValidationState.Add(value, new ValidationStateEntry
            {
                Key = modelName
            });

            bindingContext.ModelState.SetModelValue(
                modelName,
                null,
                null);

            bindingContext.Result = ModelBindingResult.Success(value);
        }

        private async Task<IAsyncEnumerable<IFormStreamedFile>> GetFormFilesAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;
            var multipartFormDataCollection = await request.ReadFormStreamedDataAsync();
            return multipartFormDataCollection.Files;
        }
    }
}