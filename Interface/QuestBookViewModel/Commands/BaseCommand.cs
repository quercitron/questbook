using System;
using System.Windows.Input;

namespace QuestBookViewModel.Commands
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            m_CanExecute = canExecute;
            m_Execute = execute;
        }

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        private readonly Func<Boolean> m_CanExecute;
        private readonly Action m_Execute;

        public void Execute(object parameter)
        {
            m_Execute();
        }

        public bool CanExecute(object parameter)
        {
            return m_CanExecute == null || m_CanExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
