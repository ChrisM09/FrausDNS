using System;
using System.Windows.Input;

namespace FrausDNS.ViewModel.Base
{
    /// <summary>
    /// Command that will run a given action.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action _action;

        /// <summary>
        /// Event that is run when the <see cref="CanExecute(object)"/> value has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Relay command can always execute.
        /// </summary>
        /// <param name="parameter">Object to be checked if it will allow the command to run.</param>
        /// <returns>True or false if the command can be executed.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Value constructor for the Relay Command
        /// </summary>
        /// <param name="action">The action that will be performed throught this relay command.</param>
        public RelayCommand(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Runs the command stored inside this command.
        /// </summary>
        /// <param name="parameter">Parameter to pass to the command.</param>
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
