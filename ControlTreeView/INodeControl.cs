using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ControlTreeView
{
    /// <summary>
    /// Provides selection logic and DragAndDrop logic for the CTreeNode.Control.
    /// </summary>
    public interface INodeControl
    {
        /// <summary>
        /// The owner node of this control.
        /// </summary>
        CTreeNode OwnerNode { set; }
    }
}