using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace UI.Helpers
{
    public class NullToVisibilityConverter : IValueConverter
    {
        // Converte: null Collapsed (nascosto), qualsiasi valore Visible
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return value == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        // Non serve la conversione inversa per questo caso
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
