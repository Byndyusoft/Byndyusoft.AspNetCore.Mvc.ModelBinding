using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    public static class FromDataBindingSource
    {
        public static readonly BindingSource Instance = new(
            "FormData",
            "FormData",
            isGreedy: false,
            isFromRequest: true);
    }
}