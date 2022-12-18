namespace Directory_Scanner.Model;

public class FileSystemTree
{
    public TreeNode? Root { get; private set; } = null;
	public FileSystemTree(TreeNode root)
	{
		Root = root;
	}
}
