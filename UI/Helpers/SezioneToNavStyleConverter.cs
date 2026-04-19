using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace UI.Helpers
{
    public class SezioneToNavStyleConverter : IValueConverter
    {
        // value         = SezioneAttiva (es. "Ristoranti")
        // parameter     = nome della voce (es. "Ristoranti")
        // Restituisce   = NavItemActiveStyle se coincidono,
        //                 NavItemStyle altrimenti
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var app = Application.Current;

            if (value?.ToString() == parameter?.ToString())
                return app.MainWindow.FindResource("NavItemActiveStyle");

            return app.MainWindow.FindResource("NavItemStyle");
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
