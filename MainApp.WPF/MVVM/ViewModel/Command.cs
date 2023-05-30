using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainApp.WPF.MVVM.ViewModel;

public sealed class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
}

public sealed class RelayCommandAsync : ICommand
{
    private readonly Func<object, Task> _execute;
    private readonly Predicate<object> _canExecute;
    private readonly Action<Exception> _onException;
    private bool _is_executing;
    public bool IsExecuting
    {
        get
        {
            return _is_executing;
        }
        set
        {
            _is_executing = value;
            //CanExecuteChanged-=
            //CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public RelayCommandAsync(Func<object, Task> execute, Predicate<object> canExecute, Action<Exception> onException)
    {
        _onException = onException;
        _canExecute = canExecute ?? ((p) => !IsExecuting);
        _execute = execute;
    }

    public async void Execute(object parameter)
    {
        IsExecuting = true;
        Task task;
        task = ExecuteAsync(parameter);
        /*try
			{
				task=ExecuteAsync(parameter);
				task.Wait();
			}
			catch (Exception ex)
			{
				_onException?.Invoke(ex);
			}*/
        task.ContinueWith(task =>
        {
            IsExecuting = false;
        });

    }
    private async Task ExecuteAsync(object parameter)
    {
        await _execute(parameter);
    }
    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
}