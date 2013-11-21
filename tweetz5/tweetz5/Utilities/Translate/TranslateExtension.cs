using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace tweetz5.Utilities.Translate
{
    [MarkupExtensionReturnType(typeof(string))]
    public class TranslateExtension : MarkupExtension
    {
        private readonly string _key;

        public TranslateExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new Translater(_key), 
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}