﻿using System;
using System.Windows.Input;

namespace Directory_Scanner.VVM.ViewModel;

// Custom event handler for MVVM
public class RelayCommand : ICommand
{
    private Action<object> _execute;
    private Predicate<object> _canExecute;
    public RelayCommand(Action<object> execute) : this(execute, null) { }
    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");
        _execute = execute; _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    public void Execute(object parameter) 
    { 
        _execute(parameter); 
    }
}
