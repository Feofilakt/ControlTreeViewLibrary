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
        ///// <summary>
        ///// Event that will be signed by CTreeNode to indicate whenever user attempts to select node.
        ///// </summary>
        //event EventHandler UserSelect;
        ///// <summary>
        ///// Event that will be signed by CTreeNode to indicate whenever user attempts to select node with some multi modificator.
        ///// </summary>
        //event EventHandler UserMultiSelect;
        ///// <summary>
        ///// Event that will be signed by CTreeNode to indicate whenever user attempts to drag nodes.
        ///// </summary>
        //event EventHandler UserDrag;

        /// <summary>
        /// 
        /// </summary>
        CTreeNode OwnerNode { set; }
    }
}