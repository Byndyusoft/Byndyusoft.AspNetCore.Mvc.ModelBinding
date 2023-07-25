using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    public static class FromStreamedDataBindingSource
    {
        public static readonly BindingSource Instance = new(
            "FromStreamedData",
            "FromStreamedData",
            isGreedy: false,
            isFromRequest: true);
    }
}