using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Extensions;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    /// <summary>
    ///     Провайдер значений для извлечения данных из формы со стримами
    /// </summary>
    public class FormStreamedDataValueProviderFactory : IValueProviderFactory
    {
        /// <inheritdoc />
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var request = context.ActionContext.HttpContext.Request;
            if (request.HasFormContentType)
            {
                // Allocating a Task only when the body is form data.
                return AddValueProviderAsync(context);
            }

            return Task.CompletedTask;
        }

        private async Task AddValueProviderAsync(
            ValueProviderFactoryContext context)
        {
            var request = context.ActionContext.HttpContext.Request;
            
            FormStreamedDataCollection formStreamedDataCollection;
            try
            {
                formStreamedDataCollection = await request.ReadFormStreamedDataAsync();
            }
            catch (InvalidDataException ex)
            {
                // ReadFormAsync can throw InvalidDataException if the form content is malformed.
                // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
                throw new ValueProviderException($"Ошибка считывания данных формы: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                // ReadFormAsync can throw IOException if the client disconnects.
                // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
                throw new ValueProviderException($"Ошибка считывания данных формы: {ex.Message}", ex);
            }

            var valueProvider = new FormStreamedDataValueProvider(
                FromStreamedDataBindingSource.Instance,
                formStreamedDataCollection,
                CultureInfo.CurrentCulture);

            context.ValueProviders.Add(valueProvider);
        }
    }
}