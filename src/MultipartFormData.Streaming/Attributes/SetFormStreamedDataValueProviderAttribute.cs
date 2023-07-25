using System;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Attributes
{
    /// <summary>
    ///     Атрибут для замены стандартной фабрики данных multipart from data на <see cref="FormStreamedDataValueProviderFactory"/>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetFormStreamedDataValueProviderAttribute : Attribute, IResourceFilter
    {
        /// <inheritdoc />
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();

            factories.Add(new FormStreamedDataValueProviderFactory());
        }

        /// <inheritdoc />
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}