using System;
using System.Windows.Input;

namespace WPFProbeklausur
{
    public class RelayCommand : ICommand
    {
        private Action<object> _action;
        private Func<object, bool> _func;
        
        public RelayCommand(Action<object> action, Func<object, bool> func)
        {
            _action = action;
            _func = func;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (_func != null)
                _func(parameter);
            return true;
        }



        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion

    }
}
