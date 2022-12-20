using Directory_Scanner.Model.Threads;
using Directory_Scanner.Model.Tree;

namespace Directory_Scanner.Model;

// Scan directory and return file system tree with calculated data as result
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

	// Condition to cancel scannig process
	public int condition = 0;

	// Files to be scanned
	public int maxFiles = 1;

	// Already scanned files
	public int currentFiles = 0;

	private object locker = new();

	private TaskQueue? _taskQueue;
	private TreeNode? _root;

	public DirectoryScanner(int maxThreads)
	{
		_root = null;
		_taskQueue = null;
		MaxThreads = maxThreads;
	}

	// Begin scanning directory
	public FileSystemTree StartScanning(string rootPath)
	{
		if (Directory.Exists(rootPath))
		{
			var dir = new DirectoryInfo(rootPath);
            _root = new TreeNode(dir.Name, rootPath, true)
            {
                Percentage = 100
            };
			lock(locker)
				maxFiles = GetTotalCount(dir);
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

	// Work submitted to queue for worker threads
	public void ScanFullDirectory(TreeNode parent)
	{
		var dir = new DirectoryInfo(parent.Path);
		if (Directory.Exists(dir.FullName))
		{
			try
			{
				// Scan files
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
                        Interlocked.Increment(ref currentFiles);
                    }
                    
                }

				// Scan directories and submit new work
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

	// Wait for worker threads to stop or cancellation request
    public void WaitForActiveWorkers()
    {
        while (_taskQueue.WaitingCount != maxThreads && condition == 0)
        {
        }
		_taskQueue.Close();
    }

	// Change condition to cancel scanning
	public void SuspendWorkers()
	{
		Interlocked.Increment(ref condition);
	}

	// Preliminary file amount evaluation
	public int GetTotalCount(DirectoryInfo root)
	{
		int totalCount = 0;
		try
		{
			totalCount = root.GetFiles().Where(file => file.LinkTarget is null).ToArray().Length;
			var directories = root.GetDirectories().Where(dir => dir.LinkTarget is null).ToList();
			foreach (var directory in directories)
			{
				totalCount += GetTotalCount(directory);
			}
		}
		catch (Exception ex)
		{
            return 0;
        }
		return totalCount;
	}

}
