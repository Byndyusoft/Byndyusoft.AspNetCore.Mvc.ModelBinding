using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    /// Specifies that a parameter or property should be bound using form-data in the request body.
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