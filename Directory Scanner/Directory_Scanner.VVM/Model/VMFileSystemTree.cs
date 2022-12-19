using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner.VVM.Model;

public class VMFileSystemTree
{
    public static string icoFolderPath = "";
    public static string icoFilePath = "";

    public VMTreeNode? Root { get; private set; } = null;
    public VMFileSystemTree(VMTreeNode root)
    {
        Root = root;
    }

}
