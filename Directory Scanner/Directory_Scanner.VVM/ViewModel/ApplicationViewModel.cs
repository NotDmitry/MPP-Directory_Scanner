using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Directory_Scanner.Model;
using System.Windows.Forms;

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
    public bool IsWorkings
    {
        get => _isWorking;
        set
        {
            _isWorking = value;
            OnPropertyChanged("IsWorking");
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
            return _openDirectory ??= new RelayCommand(obj =>
            {
                var openDialog = new FolderBrowserDialog();
                DialogResult result = openDialog.ShowDialog();
                if (result == DialogResult.OK)
                    RootPath = openDialog.SelectedPath;
            });

        }


    }

    public RelayCommand CancelScanning
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




    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
