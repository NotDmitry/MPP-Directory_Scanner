using Directory_Scanner.Model.Tree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner.VVM.Model;

public class VMTreeNode
{
    public string Name { get; set; }
    public long Size { get; set; }
    public double Percentage { get; set; }
    public string Icon { get; set; }
    public List<VMTreeNode>? Children { get; set; } = null;
    public VMTreeNode(TreeNode node)
    {
        Name = node.Name;
        Icon = node.IsDirectory ? VMFileSystemTree.icoFolderPath : VMFileSystemTree.icoFilePath;
        Size = node.Size;
        Percentage = node.Percentage;
    }

    public static VMTreeNode ConvertChildren(VMTreeNode nodeVM, TreeNode node)
    {
        if (node.Children is not null)
        {
            var newChildren  = new List<VMTreeNode>();
            foreach (var child in node.Children) 
            {
                var newVMNode = new VMTreeNode(child);
                newVMNode = ConvertChildren(newVMNode, child);
                newChildren.Add(newVMNode);
            }
            nodeVM.Children = newChildren.OrderBy(x => x.Name).ToList();
        }
        return nodeVM;

    }
}
