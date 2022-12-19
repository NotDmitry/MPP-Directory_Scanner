using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner.VVM.Model;

public class VMFileSystemTree
{
    public static string icoFolderPath = "Icons/FolderIcon.png";
    public static string icoFilePath = "Icons/FileIcon.png";

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
