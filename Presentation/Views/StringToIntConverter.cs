using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ICGFrame.Presentation.Views
{
    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, string? culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return "0"; // fallback
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, string? culture)
        {
            try
            {
                if (value is string str)
                {
                    if (string.IsNullOrWhiteSpace(str))
                        return 0;

                    if (int.TryParse(str, out int result))
                    {
                        return Math.Clamp(result, 0, 10);
                    }
                }

                return 0; // fallback при любом непонятном вводе
            }
            catch
            {
                return 0; // ещё один уровень защиты
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}