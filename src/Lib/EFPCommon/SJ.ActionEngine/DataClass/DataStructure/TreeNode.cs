using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEngine.DataClass.DataStructure
{
    public class TreeNode
    {
        public TreeNode(object val)
        {
            this.val = val;
        }

        public object val { get; set; }

        public TreeNode? left { get; set; }

        public TreeNode? right { get; set; }
    }
}
