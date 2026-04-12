using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Helpers
{
    /// <summary>
    /// Implements the ICommand interface by delegating command logic to specified delegates.
    /// Allows UI actions to be associated with view model methods in MVVM architectures.
    /// </summary>
    /// <remarks>RelayCommand is commonly used in MVVM applications 
    /// to encapsulate command logic and enable or disable UI elements
    /// based on the application state.
    /// The ability to execute a command is determined by the provided canExecute delegate, if any.
    /// The CanExecuteChanged event is automatically raised when the command handler 
    /// detects conditions that might change the ability to execute the command.</remarks>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        /// <summary>
        /// WPF calls this method to determine whether the button should be enabled or disabled.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        /// <summary>
        /// WPF calls this method when the user clicks the button.
        /// It simply executes the _execute function passed to it in the constructor.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _execute();


        /// <summary>
        /// CanExecuteChanged is the event WPF listens for to know when to reevaluate CanExecute and 
        /// update the button's enabled/disabled state.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
