using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Utility
{
    public class CustomFormatter : IFormatProvider, ICustomFormatter
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        public Dictionary<string, FormatEntry> FormattingRules;

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            _stringBuilder.Clear();

            if (string.IsNullOrEmpty(format))
                return HandleOtherFormats(format, arg);

            if (FormattingRules.TryGetValue(format, out var customFormatting))
            {
                _stringBuilder.Append(customFormatting.Prefix);
                _stringBuilder.Append(HandleOtherFormats(customFormatting.Format, arg));
                _stringBuilder.Append(customFormatting.Suffix);
                return _stringBuilder.ToString();
            }

            try
            {
                return HandleOtherFormats(format, arg);
            }
            catch (FormatException e)
            {
                throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
            }
        }

        private string HandleOtherFormats(string format, object arg)
        {
            switch (arg)
            {
                case IFormattable formattable:
                    return formattable.ToString(format, CultureInfo.CurrentCulture);
                case null:
                    return string.Empty;
                default:
                    return arg.ToString();
            }
        }
    }
}
