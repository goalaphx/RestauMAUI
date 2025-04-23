using System.Globalization;

namespace Restaurant.Converters; // Or your actual Converters namespace

public class IsGreaterThanZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check for integer type
        if (value is int intValue)
        {
            return intValue > 0;
        }
        // Check for decimal type
        if (value is decimal decValue)
        {
            return decValue > 0;
        }
        // Could add checks for other numeric types like double, float if needed
        // Default to false if the type is not handled or null
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Not usually needed for visibility conversion
        throw new NotImplementedException();
    }
}