using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.MultipartFormData.Streaming.Binders
{
    public sealed class FormStreamedDataValueProvider : BindingSourceValueProvider, IEnumerableValueProvider
    {
        public const string CultureInvariantFieldName = "__Invariant";
        public const string FilesFieldName = "Files";

        private readonly FormStreamedDataCollection _values;
        private readonly HashSet<string?>? _invariantValueKeys;
        private PrefixContainer? _prefixContainer;

        /// <summary>
        /// Creates a value provider for <see cref="IFormCollection"/>.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource"/> for the data.</param>
        /// <param name="values">The key value pairs to wrap.</param>
        /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
        public FormStreamedDataValueProvider(
            BindingSource bindingSource,
            FormStreamedDataCollection values,
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

            if (_values.Fields.TryGetValue(key, out var values) == false)
                values = StringValues.Empty;

            if (values.Count == 0)
            {
                return ValueProviderResult.None;
            }

            var culture = _invariantValueKeys?.Contains(key) == true ? CultureInfo.InvariantCulture : Culture;
            return new ValueProviderResult(values, culture);
        }
    }
}