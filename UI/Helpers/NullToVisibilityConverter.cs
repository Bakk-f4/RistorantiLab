using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UI.Helpers
{
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converte un valore null in Visibility.Collapsed e un valore non null in Visibility.Visible.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return value == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Converte un valore Visibility.Collapsed in null e 
        /// un valore Visibility.Visible in un oggetto non null (ad esempio, new object()).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
