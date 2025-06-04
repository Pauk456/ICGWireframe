using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ICGFrame.Presentation.Converters
{
    public class AllValuesValidConverter : IMultiValueConverter
    {
        public static AllValuesValidConverter Instance { get; } = new AllValuesValidConverter();

        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value == null || !(value is decimal) || (decimal)value < 1)
                    return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}