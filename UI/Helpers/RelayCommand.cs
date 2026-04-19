using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// metodo utilizzato per invocare un azione nella UI
        /// pattern C# moderno per validare gli argomenti obbligatori
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute
                          ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// WPF chiama questo metodo per sapere se il bottone deve essere abilitato o disabilitato.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
            => _canExecute?.Invoke() ?? true;

        /// <summary>
        /// WPF chiama questo metodo quando l'utente clicca il bottone. 
        /// Esegue semplicemente la funzione _execute che gli è stata passata nel costruttore. 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
            => _execute();

        /// <summary>
        /// CanExecuteChanged è l'evento che WPF ascolta per sapere 
        /// quando rivalutare CanExecute e aggiornare lo stato abilitato/disabilitato del bottone.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
