// Converters/IsNullOrEmptyConverter.cs
using System.Globalization;

namespace Restaurant.Converters; // Or your actual converters namespace

public class IsNullOrEmptyConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>False if the string is null or empty, True otherwise.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Checks if the string is null or consists only of white-space characters.
        // Adjust to string.IsNullOrEmpty(value as string) if you only want to check for null or empty, not just whitespace.
        return !string.IsNullOrWhiteSpace(value as string);
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