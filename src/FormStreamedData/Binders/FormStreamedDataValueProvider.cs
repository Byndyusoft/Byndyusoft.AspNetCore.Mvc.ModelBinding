using System;
using System.Collections.Generic;
using System.Globalization;
using Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.Binders
{
    /// <summary>
    ///     Провайдер извлеченных данных из формы (multipart form data) без считывания стримов файлов
    /// </summary>
    public sealed class FormStreamedDataValueProvider : BindingSourceValueProvider, IEnumerableValueProvider
    {
        public const string CultureInvariantFieldName = "__Invariant";
        private readonly HashSet<string?>? _invariantValueKeys;

        private readonly FormStreamedDataCollection _values;
        private PrefixContainer? _prefixContainer;

        /// <summary>
        ///     Конструктор провайдера для данных типа <see cref="FormStreamedDataCollection" />.
        /// </summary>
        /// <param name="bindingSource"><see cref="BindingSource" /> данных.</param>
        /// <param name="values">Обернутые значения формы без считывания стримов.</param>
        /// <param name="culture">Локаль для возврата результатов.</param>
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
                _invariantValueKeys = new HashSet<string?>(invariantKeys, StringComparer.OrdinalIgnoreCase);

            Culture = culture;
        }

        /// <summary>
        ///     Локаль для возврата результатов.
        /// </summary>
        public CultureInfo? Culture { get; }

        /// <summary>
        ///     Коллекция префиксов.
        /// </summary>
        private PrefixContainer PrefixContainer
        {
            get { return _prefixContainer ??= new PrefixContainer(_values.Fields.Keys); }
        }

        /// <inheritdoc />
        public override bool ContainsPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

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

            if (key.Length == 0)
                // Top level parameters will fall back to an empty prefix when the parameter name does not
                // appear in any value provider. This would result in the parameter binding to a form parameter
                // with a empty key (e.g. Request body looks like "=test") which isn't a scenario we want to support.
                // Return a "None" result in this event.
                return ValueProviderResult.None;

            if (_values.Fields.TryGetValue(key, out var values) == false)
                return ValueProviderResult.None;

            if (values.Count == 0)
                return ValueProviderResult.None;

            var culture = _invariantValueKeys?.Contains(key) == true ? CultureInfo.InvariantCulture : Culture;
            return new ValueProviderResult(values, culture);
        }
    }
}