namespace Directory_Scanner.Model.Tree;

public class TreeNode
{
    public string Name { get; set; }
    public string AbsolutePath { get; set; }
    public long Size { get; set; }
    public double Percentage { get; set; }

    private bool _isDirectory;
    public bool IsDirectory
    {
        get => _isDirectory;
        private set
        {
            _isDirectory = value;
            if (_isDirectory)
                Children = new List<TreeNode>();
        }
    }
    public List<TreeNode>? Children { get; set; } = null;
    public TreeNode(string name, string path)
    {
        Name = name;
        AbsolutePath = path;
        Size = 0;
        Percentage = 0;
    }

}
