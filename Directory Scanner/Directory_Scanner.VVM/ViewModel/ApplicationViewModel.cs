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

namespace Directory_Scanner.VVM.ViewModel;

public class ApplicationViewModel : INotifyPropertyChanged
{

    private DirectoryScanner _directoryScanner;


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

    private bool _isWorking;
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
                    IsWorking = true;
                    _directoryScanner = new DirectoryScanner(MaxThreads);
                    FileSystemTree result = _directoryScanner.StartScanning(RootPath);
                    var rootVM = new VMTreeNode(result.Root);
                    rootVM = VMTreeNode.ConvertChildren(rootVM, result.Root);
                    TreeVM = new VMFileSystemTree(rootVM);
                    IsWorking = false;
                });
            });

        }


    }

    public RelayCommand _cancelScanning;
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




    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
