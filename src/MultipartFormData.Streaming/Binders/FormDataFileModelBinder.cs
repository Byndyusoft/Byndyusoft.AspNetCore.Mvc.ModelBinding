using System.Collections.Generic;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    /// <summary>
    /// <see cref="IModelBinder"/> implementation to bind posted files to <see cref="IFormFile"/>.
    /// </summary>
    public class FormDataFileModelBinder : IModelBinder
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
            var value = new StreamFormFileCollection(files);

            // We need to add a ValidationState entry because the modelName might be non-standard. Otherwise
            //// the entry we create in model state might not be marked as valid.
            bindingContext.ValidationState.Add(value, new ValidationStateEntry()
            {
                Key = modelName,
            });

            bindingContext.ModelState.SetModelValue(
                modelName,
                rawValue: null,
                attemptedValue: null);

            bindingContext.Result = ModelBindingResult.Success(value);
        }

        private async Task<IAsyncEnumerable<MultipartFormDataFileDto>> GetFormFilesAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;
            var multipartFormDataCollection = await request.ReadFormStreamedDataAsync();
            return multipartFormDataCollection.Files;
        }
    }
}