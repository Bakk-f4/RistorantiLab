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
    /// ViewModelBase class that implements INotifyPropertyChanged to facilitate notification of property changes in ViewModels.
    /// It is an abstract class that can be inherited by any ViewModel.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        /// <summary>
        /// An event that is raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Method used to raise the PropertyChanged event when a property changes.
        /// Uses the CallerMemberName to automatically get the name of the property that called the method.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }   
    }
}
