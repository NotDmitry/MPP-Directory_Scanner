using System.Collections.Concurrent;

namespace Directory_Scanner.Model.Tree;

// File tree element represents file or directory
public class TreeNode
{
    public string Name { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    
    // Relative size according to parent directory
    public double Percentage { get; set; }

    private bool _isDirectory;
    public bool IsDirectory
    {
        get => _isDirectory;
        private set
        {
            _isDirectory = value;
            if (_isDirectory)
                Children = new ConcurrentBag<TreeNode>();
        }
    }
    public ConcurrentBag<TreeNode>? Children { get; set; } = null;
    public TreeNode(string name, string path, bool isDirectory)
    {
        Name = name;
        Path = path;
        IsDirectory = isDirectory;
        Size = 0;
        Percentage = 0.0;
    }

    // Calculate relative size for all elements (including nested)
    public void CalculatePercentage()
    {
        if (IsDirectory && Children is not null)
        {
            foreach (TreeNode child in Children)
            {
                if (Size > 0)
                    child.Percentage = (double)child.Size/ Size * 100;
                child.CalculatePercentage();
            }
        }
    }

    // Calculate size in bytes for all elements (including nested)
    public long CalculateSize()
    {
        if (IsDirectory && Children is not null) 
        { 
            Size = Children.Select(child => child.CalculateSize()).Sum();
        }
        return Size;
    }

}
