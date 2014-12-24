using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ControlTreeView
{
    public class DragNodesEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragNodeEventArgs class.
        /// </summary>
        /// <param name="nodes">The tree nodes that the event is responding to.</param>
        public DragNodesEventArgs(List<CTreeNode> nodes)
        {
            Nodes=nodes;
        }

        /// <summary>
        /// Gets the tree nodes that have been dragged.
        /// </summary>
        public List<CTreeNode> Nodes { get; private set; }
    }
}
