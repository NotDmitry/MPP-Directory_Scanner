using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Directory_Scanner.Model;
using System.Windows.Forms;
using Directory_Scanner.VVM.Model;
using Directory_Scanner.Model.Tree;

namespace Directory_Scanner.VVM.ViewModel;

// Data and commands for binding to UI
public class ApplicationViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private DirectoryScanner _directoryScanner;
    public ApplicationViewModel() { }

    // Progress bar data
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

    // Maximal amount of working threads for scanning
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

    // Root directory for scanning
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

    // Additional data for changing progress status
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

    // Check if scanning in progress
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

    // Result tree
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

    // Open folder dialog to choose directory
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

    // Beging scanning in selected directory
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

    // Cancel scanning if in progress
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
