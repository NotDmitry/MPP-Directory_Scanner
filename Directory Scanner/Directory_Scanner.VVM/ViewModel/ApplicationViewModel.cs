using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Directory_Scanner.Model;
using System.Windows.Forms;
using Directory_Scanner.VVM.Model;
using Directory_Scanner.Model.Tree;
using System.Drawing;
using System.Threading;

namespace Directory_Scanner.VVM.ViewModel;

public class ApplicationViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private DirectoryScanner _directoryScanner;
    public ApplicationViewModel() { }

    private double _taskProgress = 0.0;
    public double TaskProgress
    {
        get => _taskProgress;
        set
        {
            _taskProgress = value;
            OnPropertyChanged("TaskProgress");
        }
    }

    private int _maxThreads = 16;
    public int MaxThreads
    {
        get => _maxThreads;
        set
        {
            _maxThreads = value;
            OnPropertyChanged("MaxThreads");
        }
    }

    private string? _rootPath = "";
    public string? RootPath
    {
        get => _rootPath;
        set
        {
            _rootPath = value;
            OnPropertyChanged("RootPath");
        }
    }

    private string? _status = "Directory: ";
    public string? Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged("Status");
        }
    }

    private bool _isWorking = false;
    public bool IsWorking
    {
        get => _isWorking;
        set
        {
            _isWorking = value;
            OnPropertyChanged("IsWorking");
        }
    }

    private VMFileSystemTree _treeVM;
    public VMFileSystemTree TreeVM
    {
        get => _treeVM;

        private set
        {
            _treeVM = value;
            OnPropertyChanged("TreeVM");
        }
    }


    private RelayCommand _openDirectory;
    public RelayCommand OpenDirectory
    {
        get
        {
            return _openDirectory ??= new RelayCommand(obj =>
                {
                    var openDialog = new FolderBrowserDialog();
                    DialogResult result = openDialog.ShowDialog();
                    if (result == DialogResult.OK)
                        RootPath = openDialog.SelectedPath;
                    Status = "Directory: " + RootPath;
                });

        }

    }

    private RelayCommand _startScanning;
    public RelayCommand StartScanning
    {
        get
        {
            return _startScanning ??= new RelayCommand(obj =>
            {
                Task.Run(() =>
                {
                    Status = "Scanning... " + RootPath;
                    IsWorking = true;
                    _directoryScanner = new DirectoryScanner(MaxThreads);
                    Task.Run(() =>
                    {
                        while (IsWorking)
                        {
                            TaskProgress = Math.Round((double)_directoryScanner.currentFiles / _directoryScanner.maxFiles * 100);
                        }
                        TaskProgress = 100;
                        Status = "Complete: " + RootPath;
                    });
                    FileSystemTree result = _directoryScanner.StartScanning(RootPath);
                    var rootVM = new VMTreeNode(result.Root);
                    rootVM = VMTreeNode.ConvertChildren(rootVM, result.Root);
                    TreeVM = new VMFileSystemTree(rootVM);
                    IsWorking = false;
                });

            });

        }


    }

    private RelayCommand _cancelScanning;
    public RelayCommand CancelScanning
    {
        get
        {
            return _cancelScanning ??= new RelayCommand(obj =>
            {
                if (IsWorking) 
                {
                    _directoryScanner.SuspendWorkers();
                    IsWorking = false;
                }
            });

        }


    }

}
