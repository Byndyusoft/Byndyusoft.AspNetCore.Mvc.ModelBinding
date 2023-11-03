// ReSharper disable CheckNamespace

using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        ///     Регистрация класса для биндинга <see cref="FormStreamedFileCollection" />
        /// </summary>
        public static MvcOptions AddFormStreamedFileCollectionBinder(this MvcOptions mvcOptions)
        {
            mvcOptions.ModelBinderProviders.Insert(0, new FormStreamedFileModelBinderProvider());
            return mvcOptions;
        }
    }
}