using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Extensions;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     Фабрика провайдера извлеченных данных из формы (multipart form data) без считывания стримов файлов
    /// </summary>
    public class FormStreamedDataValueProviderFactory : IValueProviderFactory
    {
        /// <inheritdoc />
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var request = context.ActionContext.HttpContext.Request;

            // Allocating a Task only when the body is form data.
            if (request.HasFormContentType)
                return AddValueProviderAsync(context);

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