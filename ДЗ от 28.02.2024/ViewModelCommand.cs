using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public sealed class ViewModelCommand : ModelCommand
    {
        private Func<bool> _canExecute;
        private Action _execute;

        public ViewModelCommand(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        protected override bool CanExecute()
        {
            return _canExecute.Invoke();
        }

        protected override void Execute()
        {
            if (_execute != null)
                _execute.Invoke();
        }
    }
}
