using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UI.Helpers
{
    /// <summary>
    /// Classe astratta per l' implementazione di INotifyPropertyChanged
    /// Questa classe verra' ereditata dagli altri view models per utilizzare INotifyPropertyChanged
    /// aggiornando i dati nella UI
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Metodo che serve per notifiare tutti gli "ascoltatori"
        /// CallerMemberName imposta in automatico il nome del controllo da aggiornare
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
