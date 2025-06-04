using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ICGFrame.Presentation
{
    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, string? culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return "0";
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, string? culture)
        {
            if (value is string str && int.TryParse(str, out int result))
            {
                return Math.Clamp(result, 0, 10);
            }

            return 0; // fallback, если не число или вне диапазона
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}