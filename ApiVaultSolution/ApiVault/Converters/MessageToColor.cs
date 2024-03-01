using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia.Media;


namespace ApiVault.Converters
{
    public class MessageToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string message))
                return Brushes.Transparent;

            // Define custom colors
            Color successColor = Color.Parse("#28A745"); // A green shade
            Color errorColor = Color.Parse("#DC3545"); // A red shade

            // Use custom colors based on the message content
            return message.Contains("success") ? new SolidColorBrush(successColor) : new SolidColorBrush(errorColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
