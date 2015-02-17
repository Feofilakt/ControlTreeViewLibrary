using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

namespace ControlTreeView
{
    /// <summary>
    /// Represents a node of a CTreeView.
    /// </summary>
    public partial class CTreeNode : INodeContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CTreeNode class with the default control.
        /// </summary>
        public CTreeNode()
            : this("", new Control()) { }

        /// <summary>
        /// Initializes a new instance of the CTreeNode class with the specified CTreeNodeControl.
        /// </summary>
        /// <param name="control">Сontrol that will be assigned to this node.</param>
        public CTreeNode(Control control)
            : this("", control) { }

        /// <summary>
        /// Initializes a new instance of the CTreeNode class with the specified name and control.
        /// </summary>
        /// <param name="name"></param>
        public CTreeNode(string name, Control control)
        {
            Nodes = new CTreeNodeCollection(this);
            Level = 0;
            Visible = true;
            IsExpanded = true;
            Name = name;
            Control = control;
        }
        #endregion

        #region Properties
        private Control _Control;
        /// <summary>
        /// Gets or sets the user control assigned to the current tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public Control Control
        {
            get { return _Control; }
            set
            {
                bool notNull = (OwnerCTreeView != null);
                if (notNull)
                {
                    OwnerCTreeView.Controls.Remove(_Control);
                    OwnerCTreeView.Controls.Add(value);
                }
                _Control = value;
                if (value is INodeControl)((INodeControl)_Control).OwnerNode = this;
                if (notNull) OwnerCTreeView.Recalculate();
            }
        }

        /// <summary>
        /// Gets the collection of CTreeNode objects assigned to the current tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public CTreeNodeCollection Nodes { get; private set; }

        /// <summary>
        /// Gets the parent tree node of the current tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public CTreeNode ParentNode { get; internal set; }

        /// <summary>
        /// Gets the parent INodeContainer of the current tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public INodeContainer Parent
        {
            get
            {
                if (ParentNode != null) return ParentNode;
                else return OwnerCTreeView;
            }
        }

        /// <summary>
        /// Gets or sets the name of the tree node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the position of the tree node in the tree node collection.
        /// </summary>
        //[BrowsableAttribute(false)]
        public int Index
        {
            get
            {
                if (Parent != null) return Parent.Nodes.IndexOf(this);
                else return -1;
            }
        }

        private CTreeView _OwnerCTreeView;
        /// <summary>
        /// Gets the parent tree view that the tree node is assigned to.
        /// </summary>
        [BrowsableAttribute(false)]
        public CTreeView OwnerCTreeView
        {
            get { return _OwnerCTreeView; }
            internal set
            {
                TraverseNodes(node => { node._OwnerCTreeView = value; });
            }
        }
        //{
        //    get
        //    {
        //        if (Parent != null) return Parent.CTreeView;
        //        else return _CTreeView;
        //    }
        //    internal set { _CTreeView = value; }
        //}

        /// <summary>
        /// Gets or sets the object that contains data about the tree node.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the next sibling tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public CTreeNode NextNode
        {
            get
            {
                if (Index >= 0 && Index < Parent.Nodes.Count - 1) return Parent.Nodes[Index + 1];
                else return null;
            }
        }

        /// <summary>
        /// Gets the previous sibling tree node.
        /// </summary>
        [BrowsableAttribute(false)]
        public CTreeNode PrevNode
        {
            get
            {
                if (Index > 0) return Parent.Nodes[Index - 1];
                else return null;
            }
        }

        /// <summary>
        /// Gets the first child tree node in the tree node collection.
        /// </summary>
        /// <value>The first child CTreeNode in the Nodes collection.</value>
        [BrowsableAttribute(false)]
        public CTreeNode FirstNode
        {
            get
            {
                if (Nodes.Count > 0) return Nodes[0];
                else return null;
            }
        }

        /// <summary>
        /// Gets the last child tree node in the tree node collection.
        /// </summary>
        /// <value>A CTreeNode that represents the last child tree node.</value>
        [BrowsableAttribute(false)]
        public CTreeNode LastNode
        {
            get
            {
                if (Nodes.Count > 0) return Nodes[Nodes.Count - 1];
                else return null;
            }
        }

        private int _Level;
        /// <summary>
        /// Gets the zero-based depth of the tree node in the CTreeView.
        /// </summary>
        //[BrowsableAttribute(false)]
        public int Level
        {
            get { return _Level; }
            internal set
            {
                _Level = value;
                Nodes.TraverseNodes(node => { node._Level = node.ParentNode.Level + 1; });
            }
        }

        /// <summary>
        /// Gets the path from the root tree node to the current tree node.
        /// </summary>
        /// <value>The path from the root tree node to the current tree node.</value>
        public string FullPath
        {
            get
            {
                if (OwnerCTreeView == null) throw new InvalidOperationException("The node is not contained in a CTreeView.");
                if (Level == 0) return Name;
                else return ParentNode.FullPath + OwnerCTreeView.PathSeparator + Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tree node is in the expanded state.
        /// </summary>
        //[BrowsableAttribute(false)]
        public bool IsExpanded { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tree node is in the selected state.
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsSelected
        {
            get
            {
                if (OwnerCTreeView != null)return OwnerCTreeView.SelectedNodes.Contains(this);
                else return false;
            }
            set
            {
                if (OwnerCTreeView == null) throw new InvalidOperationException("The node is not contained in a CTreeView.");
                else
                {
                    if (value == true)
                    {
                        OwnerCTreeView._selectedNodes.Add(this);
                        OwnerCTreeView.OnSelectNode(new CTreeViewEventArgs(this));
                    }
                    else
                    {
                        OwnerCTreeView._selectedNodes.Remove(this);
                        //ParentCTreeView.Invalidate();
                        //ParentCTreeView.Update();
                        OwnerCTreeView.Refresh();
                    }
                }
            }
        }

        private bool _Visible;
        /// <summary>
        /// Gets a value indicating whether the tree node is not hidden by its ancestors.
        /// </summary>
        //[BrowsableAttribute(false)]
        public bool Visible
        {
            get { return _Visible; }
            internal set
            {
                _Visible = value;
                //if (value) Nodes.TraverseNodes(node => node.ParentNode.IsExpanded, node => { node._Visible = true; });
                //else Nodes.TraverseNodes(node => node.Visible, node => { node._Visible = false; });
            }
        }
        //{
        //    get
        //    {
        //        if (Parent == null) return true;
        //        else if (!Parent.IsExpanded) return false;
        //        else return Parent.IsShown;
        //    }
        //}

        /// <summary>
        /// Gets the bounds of the tree node.
        /// </summary>
        /// <value>The Rectangle that represents the bounds of the node.</value>
        //[BrowsableAttribute(false)]
        public Rectangle Bounds
        {
            get
            {
                if (!Visible) return Rectangle.Empty;
                else return new Rectangle(Location, Size);
            }
        }

        private Rectangle _boundsSubtree;
        /// <summary>
        /// Gets the bounds of the tree node includes all tree nodes indirectly rooted at this tree node.
        /// </summary>
        /// <value>The Rectangle that represents the bounds of the node's subtree.</value>
        //[BrowsableAttribute(false)]
        public Rectangle BoundsSubtree
        {
            get
            {
                if (!Visible) return Rectangle.Empty;
                else return _boundsSubtree;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Expands the tree node.
        /// </summary>
        public void Expand()
        {
            IsExpanded = true;
            //if (Visible) foreach (CTreeNode child in Nodes) child.Visible = true;
            //Nodes.TraverseNodes(node => node.ParentNode.IsExpanded, node => { node.IsShown = true; });
            if (OwnerCTreeView != null) OwnerCTreeView.OnExpandNode(new CTreeViewEventArgs(this));
        }

        /// <summary>
        /// Collapses the tree node.
        /// </summary>
        public void Collapse()
        {
            IsExpanded = false;
            //foreach (CTreeNode child in Nodes) child.Visible = false;
            //Nodes.TraverseNodes(node => node.IsShown, node => { node.IsShown = false; });
            if (OwnerCTreeView != null) OwnerCTreeView.OnCollapseNode(new CTreeViewEventArgs(this));
        }

        /// <summary>
        /// Expands the CTreeNode and all the child tree nodes.
        /// </summary>
        public void ExpandAll()
        {
            BeginUpdateCTreeView();
            TraverseNodes(node => { node.Expand(); });
            EndUpdateCTreeView();
        }

        /// <summary>
        /// Collapses the CTreeNode and all the child tree nodes.
        /// </summary>
        public void CollapseAll()
        {
            BeginUpdateCTreeView();
            TraverseNodes(node => { node.Collapse(); });
            EndUpdateCTreeView();
        }

        /// <summary>
        /// Toggles the tree node to either the expanded or collapsed state.
        /// </summary>
        public void Toggle()
        {
            if (IsExpanded) Collapse();
            else Expand();
        }

        //?
        public void Drag()
        {
            if (OwnerCTreeView.DragAndDropMode != CTreeViewDragAndDropMode.Nothing && IsSelected)
            {
                //Checking that all selected nodes has same parent
                bool checkSameParent = true;
                foreach (CTreeNode selectedNode in OwnerCTreeView.SelectedNodes)
                {
                    if (selectedNode.ParentNode != OwnerCTreeView.SelectedNodes[0].ParentNode) checkSameParent = false;
                }
                //Prepare and sort the dragged nodes
                if (checkSameParent)
                {
                    List<CTreeNode> draggedNodes = new List<CTreeNode>(OwnerCTreeView.SelectedNodes);
                    draggedNodes.Sort(new System.Comparison<CTreeNode>(new Func<CTreeNode, CTreeNode, int>((node1, node2) => node1.Index - node2.Index)));
                    OwnerCTreeView.DoDragDrop(draggedNodes, DragDropEffects.All);
                }
            }
        }

        /// <summary>
        /// Returns the number of child tree nodes.
        /// </summary>
        /// <param name="includeSubTrees">true  if the resulting count includes all tree nodes indirectly rooted at this tree node; otherwise, false.</param>
        /// <returns>The number of child tree nodes assigned to the Nodes collection.</returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            if (includeSubTrees)
            {
                int result = 0;
                foreach (CTreeNode child in Nodes) result += child.GetNodeCount(true);
                return result;
            }
            else return Nodes.Count;
        }

        /// <summary>
        /// Apply action to this node and recursively to each child node.
        /// </summary>
        /// <param name="action">Action will be applied to the nodes.</param>
        public void TraverseNodes(Action<CTreeNode> action)
        {
            action(this);
            foreach (CTreeNode childNode in Nodes) childNode.TraverseNodes(action);
        }

        /// <summary>
        /// Apply action to this node and recursively to each child node until the condition is true.
        /// </summary>
        /// <param name="condition">Condition that must be satisfied node.</param>
        /// <param name="action">Action will be applied to the nodes.</param>
        public void TraverseNodes(Func<CTreeNode, bool> condition, Action<CTreeNode> action)
        {
            if (condition(this))
            {
                action(this);
                foreach (CTreeNode childNode in Nodes) childNode.TraverseNodes(condition, action);
            }
        }
        #endregion
    }
}