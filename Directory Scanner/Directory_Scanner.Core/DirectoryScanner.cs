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
			var file = new FileInfo(rootPath);
		}
		else
			throw new Exception("This directory does not exist");

		return new FileSystemTree(_root);
	}


}
