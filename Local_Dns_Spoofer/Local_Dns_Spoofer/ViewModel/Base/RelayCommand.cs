using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Local_Dns_Spoofer.ViewModel.Base
{
    public class RelayCommand : ICommand
    {
        private Action _action;


        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public RelayCommand(Action action)
        {
            _action = action;
        }


        public void Execute(object parameter)
        {
            _action();
        }



    }
}
