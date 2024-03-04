using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1
{
    public abstract class ModelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected virtual bool CanExecute()
        {
            return true;
        }

        public void RaiseCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        protected abstract void Execute();

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }
    }
}
