using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlTreeView
{
    public class CTreeViewEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the CTreeViewEventArgs class for the specified tree node.
        /// </summary>
        /// <param name="node">The CTreeNode that the event is responding to.</param>
        public CTreeViewEventArgs(CTreeNode node)
        {
            Node = node;
        }

        /// <summary>
        /// Gets the tree node that has been expanded, collapsed, or selected.
        /// </summary>
        public CTreeNode Node { get; private set; }
    }
}
