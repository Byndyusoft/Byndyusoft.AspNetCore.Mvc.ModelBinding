using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     <see cref="BindingSource" /> для считывания данных из формы form-data без считывания стримов файлов.
    /// </summary>
    public static class FromStreamedDataBindingSource
    {
        public static readonly BindingSource Instance = new(
            "FromStreamedData",
            "FromStreamedData",
            false,
            true);
    }
}