using Directory_Scanner.Model.Threads;
using Directory_Scanner.Model.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner.Model;

public class DirectoryScanner
{
	private int maxThreads;
	private readonly int _defaultThreads = 6;
    public int MaxThreads 
	{
		get => maxThreads;
		private set
		{
			maxThreads = value > 0 ? value : _defaultThreads; 
		}
	}

	public int condition = 0;

	private TaskQueue? _taskQueue;
	private TreeNode? _root;

	public DirectoryScanner(int maxThreads)
	{
		_root = null;
		_taskQueue = null;
		MaxThreads = maxThreads;
	}

	public FileSystemTree StartScanning(string rootPath)
	{
		if (Directory.Exists(rootPath))
		{
			var dir = new DirectoryInfo(rootPath);
            _root = new TreeNode(dir.Name, rootPath, true)
            {
                Percentage = 100
            };
            _taskQueue = new TaskQueue(maxThreads);
			_taskQueue.EnqueueTask(() => ScanFullDirectory(_root));
		}
		else
			throw new Exception("This directory does not exist");

		WaitForActiveWorkers();
		_root.CalculateSize();
		_root.CalculatePercentage();
		return new FileSystemTree(_root);
	}

	public void ScanFullDirectory(TreeNode parent)
	{
		var dir = new DirectoryInfo(parent.Path);
		if (Directory.Exists(dir.FullName))
		{
			try
			{
				FileInfo[] files = dir.GetFiles();
				foreach (FileInfo file in files) 
				{ 
					if (file.LinkTarget is null && !_taskQueue.Cts.Token.IsCancellationRequested)
					{
                        var insertionNode = new TreeNode(file.Name, file.FullName, false)
                        {
                            Size = file.Length
                        };
                        parent.Children?.Add(insertionNode);
					}
				}

                DirectoryInfo[] directories = dir.GetDirectories();
                foreach (DirectoryInfo directory in directories)
                {
                    if (directory.LinkTarget is null && directory.Exists && !_taskQueue.Cts.Token.IsCancellationRequested)
                    {
                        var insertionNode = new TreeNode(directory.Name, directory.FullName, true);
                        parent.Children?.Add(insertionNode);
						_taskQueue?.EnqueueTask(() => ScanFullDirectory(insertionNode));
                    }
                }

            }
			catch(Exception ex) 
			{
			}
		}
		else
			throw new Exception("This directory does not exist");
	}

    public void WaitForActiveWorkers()
    {
        while (_taskQueue.WaitingCount != maxThreads && condition == 0)
        {
        }
		_taskQueue.Close();
    }

	public void SuspendWorkers()
	{
		Interlocked.Increment(ref condition);
	}

}
