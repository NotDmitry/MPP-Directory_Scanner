using System.Collections.Generic;

namespace Directory_Scanner.VVM.Model;

public class VMFileSystemTree
{
    public static string icoFolderPath = System.IO.Path.GetFullPath(@"..\..\..\Icons\FolderIcon.png");
    public static string icoFilePath = System.IO.Path.GetFullPath(@"..\..\..\Icons\FileIcon.png");

    public VMTreeNode? Root { get; private set; } = null;
    public List<VMTreeNode> Children { get; set; } = null;
    public VMFileSystemTree(VMTreeNode root)
    {
        Root = root;
        Children = new List<VMTreeNode>
        {
            root
        };
    }

}
