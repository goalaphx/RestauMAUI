using System.Globalization;

namespace Restaurant.Converters; // Or your actual converters namespace

public class IsNotNullConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The object value to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>True if the value is not null, False otherwise.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    /// <summary>
    /// Converts a value back. Not implemented.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Not needed for one-way binding used for visibility
        throw new NotImplementedException();
    }
}