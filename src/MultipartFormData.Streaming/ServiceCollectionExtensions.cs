using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        ///     Добавляет в зависимости службу вычитки контента из multi part form data
        /// </summary>
        public static IServiceCollection AddMultipartFormDataFileProvider(this IServiceCollection services)
        {
            services.AddSingleton<IMultipartFormDataFileProvider, MultipartFormDataFileProvider>();
            return services;
        }
    }
}