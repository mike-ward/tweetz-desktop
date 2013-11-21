using System.Collections.Generic;
using System.Globalization;

namespace tweetz5.Utilities.Translate
{
    public interface ITranslationProvider
    {
        IEnumerable<CultureInfo> Languages { get; }
        object Translate(string key);
    }
}