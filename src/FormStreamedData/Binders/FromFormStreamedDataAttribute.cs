using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     Обозначает, что параметр или свойство должно быть привязано к полю из form-data в теле запроса.
    ///     Стримы файлов считываются пользователем после биндинга данных.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class FromFormStreamedDataAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        /// <inheritdoc />
        public BindingSource BindingSource => FromStreamedDataBindingSource.Instance;

        /// <inheritdoc />
        public string? Name { get; set; }
    }
}