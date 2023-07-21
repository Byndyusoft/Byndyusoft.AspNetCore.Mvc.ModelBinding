using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Dtos;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming
{
    /// <summary>
    ///     Провайдер значений для извлечения данных из формы со стримами
    /// </summary>
    public class FormDataValueProviderFactory : IValueProviderFactory
    {
        private readonly IMultipartFormDataFileProvider _multipartFormDataFileProvider;

        public FormDataValueProviderFactory(IMultipartFormDataFileProvider multipartFormDataFileProvider)
        {
            _multipartFormDataFileProvider = multipartFormDataFileProvider;
        }

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

        private async Task AddValueProviderAsync(ValueProviderFactoryContext context)
        {
            var request = context.ActionContext.HttpContext.Request;

            MultipartFormDataCollection multipartFormDataCollection;
            try
            {
                multipartFormDataCollection = await _multipartFormDataFileProvider.GetFormDataAsync(request, CancellationToken.None);
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

            var valueProvider = new FormDataValueProvider(
                FromDataBindingSource.Instance,
                multipartFormDataCollection,
                CultureInfo.CurrentCulture);

            context.ValueProviders.Add(valueProvider);
        }
    }

    public static class FromDataBindingSource
    {
        public static readonly BindingSource Instance = new BindingSource(
            "FormData",
            "FormData",
            isGreedy: true,
            isFromRequest: true);
    }

    public sealed class FormDataValueProvider : BindingSourceValueProvider, IEnumerableValueProvider
    {
        public const string CultureInvariantFieldName = "__Invariant";
        public const string FilesFieldName = "Files";

        private readonly MultipartFormDataCollection _values;
        private readonly HashSet<string?>? _invariantValueKeys;
        private PrefixContainer? _prefixContainer;

        /// <summary>
        /// Creates a value provider for <see cref="IFormCollection"/>.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource"/> for the data.</param>
        /// <param name="values">The key value pairs to wrap.</param>
        /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
        public FormDataValueProvider(
            BindingSource bindingSource,
            MultipartFormDataCollection values,
            CultureInfo? culture)
            : base(bindingSource)
        {
            if (bindingSource is null)
                throw new ArgumentNullException(nameof(bindingSource));

            _values = values ?? throw new ArgumentNullException(nameof(values));

            if (_values.Fields.TryGetValue(CultureInvariantFieldName, out var invariantKeys) && invariantKeys.Count > 0)
            {
                _invariantValueKeys = new(invariantKeys, StringComparer.OrdinalIgnoreCase);
            }

            Culture = culture;
        }

        /// <summary>
        /// The culture to use.
        /// </summary>
        public CultureInfo? Culture { get; }

        /// <summary>
        /// The prefix container.
        /// </summary>
        private PrefixContainer PrefixContainer
        {
            get
            {
                var keyCollection = _values.Fields.Keys.Concat(new[] { FilesFieldName }).ToArray();
                return _prefixContainer ??= new PrefixContainer(keyCollection);
            }
        }

        /// <inheritdoc />
        public override bool ContainsPrefix(string prefix)
        {
            return PrefixContainer.ContainsPrefix(prefix);
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetKeysFromPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

            return PrefixContainer.GetKeysFromPrefix(prefix);
        }

        /// <inheritdoc />
        public override ValueProviderResult GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (key.Equals(FilesFieldName))
                return ValueProviderResult.None;

            if (key.Length == 0)
            {
                // Top level parameters will fall back to an empty prefix when the parameter name does not
                // appear in any value provider. This would result in the parameter binding to a form parameter
                // with a empty key (e.g. Request body looks like "=test") which isn't a scenario we want to support.
                // Return a "None" result in this event.
                return ValueProviderResult.None;
            }

            var values = _values.Fields[key];
            if (values.Count == 0)
            {
                return ValueProviderResult.None;
            }

            var culture = _invariantValueKeys?.Contains(key) == true ? CultureInfo.InvariantCulture : Culture;
            return new ValueProviderResult(values, culture);
        }
    }
}