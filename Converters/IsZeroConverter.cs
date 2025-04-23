using System.Globalization;

namespace Restaurant.Converters;

public class IsZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue == 0;
        }
        if (value is decimal decValue)
        {
            return decValue == 0;
        }
        return false; // Or handle other types as needed
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}