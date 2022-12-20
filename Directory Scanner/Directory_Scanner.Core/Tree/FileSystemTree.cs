namespace Directory_Scanner.Model.Tree;

// File system from root element
public class FileSystemTree
{
    public TreeNode? Root { get; private set; } = null;
    public FileSystemTree(TreeNode root)
    {
        Root = root;
    }
}
