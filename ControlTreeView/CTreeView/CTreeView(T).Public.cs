using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace CTreeView
{
    [Designer(typeof(CTreeViewDesigner))]
    public partial class CTreeView<T> : ScrollableControl, INodeContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CTreeView<T> class.
        /// </summary>
        public CTreeView()
            : base()
        {
            suspendUpdateCount = 0;
            //InitializeComponent();
            //SuspendRecalculating = true;
            //DuringUpdating = true;
            //SuspendUpdate = true;
            BeginUpdate();
            Nodes = new CTreeNodeCollection<T>(this);
            PathSeparator=@"\";
            //DragAndDrop = new DragAndDropView();
            AutoScroll = true;
            AllowDrop = true;//?
            //DrawStyle = CTreeViewDrawStyle.Tree;
            ShowPlusMinus = true;
            ShowLines = true;
            //ShowControls = CTreeViewShowControls.Automatically;
            ShowRootLines = true;
            _selectedNodes = new List<CTreeNode>();
            
            RelocateCollapsing = true;
            SelectionMode = CTreeViewSelectionMode.Multi;
            DragAndDropMode = CTreeViewDragAndDropMode.ReplaceReorder;
            //DragAndDropFlag = false;
            IndentDepth = 30;
            IndentWidth = 10;
            selectionPen = new Pen(Color.Black, 1.0F);
            selectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            selectionBrush = new SolidBrush(SystemColors.Highlight);
            _LinesPen = new Pen(Color.Black, 1.0F);
            _LinesPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Bitmap imagePlus = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("CTreeView.Resources.plus.bmp"));
            Bitmap imageMinus = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("CTreeView.Resources.minus.bmp"));
            PlusMinus = new CTreeViewPlusMinus(imagePlus, imageMinus);
            dragDestination = new DragDestination(this);

            this.DoubleBuffered = true;
            //this.ResizeRedraw = true;
            //this.AutoScrollMinSize = new Size(0, 0);
            //DuringUpdating = false;
            //SuspendRecalculating = false;
            //SuspendUpdate = false;
            EndUpdate();
        }
        #endregion

        #region Properties

        [BrowsableAttribute(false)]
        public CTreeView<T> ParentCTreeView
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the collection of tree nodes that are assigned to the CTreeView<T> control.
        /// </summary>
        //[EditorAttribute(typeof(CTreeViewEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [BrowsableAttribute(false)]
        public CTreeNodeCollection<T> Nodes { get; private set; }

        /// <summary>
        /// Gets or sets the delimiter string that the tree node path uses.
        /// </summary>
        /// <value>The delimiter string that the tree node CTreeNode.FullPath property uses. The default is the backslash character (\).</value>
        [DefaultValue(@"\")]
        public string PathSeparator { get; set; }

        /// <summary>
        /// Gets the first child tree node in the tree node collection.
        /// </summary>
        /// <value>The first child CTreeNode<T> in the Nodes collection.</value>
        [BrowsableAttribute(false)]
        public CTreeNode<T> FirstNode
        {
            get { return Nodes[0]; }
        }

        /// <summary>
        /// Gets the last child tree node in the tree node collection.
        /// </summary>
        /// <value>A CTreeNode<T> that represents the last child tree node.</value>
        [BrowsableAttribute(false)]
        public CTreeNode<T> LastNode
        {
            get { return Nodes[Nodes.Count - 1]; }
        }

        internal List<CTreeNode> _selectedNodes;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<CTreeNode> SelectedNodes
        {
            get { return _selectedNodes.AsReadOnly(); }
        }
        internal Pen selectionPen;
        internal SolidBrush selectionBrush;

        private CTreeViewDrawStyle _DrawStyle;
        /// <summary>
        /// Gets or sets way that will draw the CTreeView.
        /// </summary>
        public CTreeViewDrawStyle DrawStyle
        {
            get { return _DrawStyle; }
            set
            {
                if (_DrawStyle != value)
                {
                    _DrawStyle = value;
                    //this.AutoScrollPosition = new Point(0, 0);
                    this.Recalculate();
                }
            }
        }

        private CTreeViewPlusMinus _PlusMinus;
        /// <summary>
        /// Gets or sets a bitmaps for plus-sign (+) and minus-sign (-) buttons of this CTreeView.
        /// </summary>
        [Browsable(false)]
        public CTreeViewPlusMinus PlusMinus
        {
            get { return _PlusMinus; }
            set
            {
                    _PlusMinus = value;
                    //this.Recalculate();
                    Refresh();
            }
        }

        private bool _ShowPlusMinus;
        /// <summary>
        /// Gets or sets a value indicating whether plus-sign (+) and minus-sign (-) buttons are displayed next to tree nodes that contain child tree nodes.
        /// </summary>
        /// <value>true  if plus sign and minus sign buttons are displayed next to tree nodes that contain child tree nodes; otherwise, false. The default is true.</value>
        [DefaultValue(true)]
        public bool ShowPlusMinus
        {
            get { return _ShowPlusMinus; }
            set
            {
                if (_ShowPlusMinus != value)
                {
                    _ShowPlusMinus = value;
                    Recalculate();
                }
            }
        }

        private bool _ShowLines;
        /// <summary>
        /// Gets or sets a value indicating whether lines are drawn between tree nodes in the tree view control.
        /// </summary>
        /// <value>true  if lines are drawn between tree nodes in the tree view control; otherwise, false. The default is true.</value>
        [DefaultValue(true)]
        public bool ShowLines
        {
            get { return _ShowLines; }
            set
            {
                if (_ShowLines != value)
                {
                    _ShowLines = value;
                    Recalculate();
                }
            }
        }

        private bool _ShowRootLines;
        /// <summary>
        /// Gets or sets a value indicating whether lines are drawn between the tree nodes that are at the root of the tree view.
        /// </summary>
        /// <value>true  if lines are drawn between the tree nodes that are at the root of the tree view; otherwise, false. The default is true.</value>
        [DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return _ShowRootLines; }
            set
            {
                if (_ShowRootLines != value)
                {
                    _ShowRootLines = value;
                    Recalculate();
                }
            }
        }

        private bool _RelocateCollapsing;
        /// <summary>
        /// Gets or sets a value indicating whether position of nodes recalculated when collapsing for diagram style of this CTreeView.
        /// </summary>
        [DefaultValue(true)]
        public bool RelocateCollapsing
        {
            get { return _RelocateCollapsing; }
            set
            {
                if (_RelocateCollapsing != value)
                {
                    _RelocateCollapsing = value;
                    Recalculate();
                }
            }
        }

        private CTreeViewSelectionMode _SelectionMode;
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(typeof(CTreeViewSelectionMode), "Multi")]
        public CTreeViewSelectionMode SelectionMode
        {
            get { return _SelectionMode; }
            set
            {
                if (_SelectionMode != value)
                {
                    _SelectionMode = value;
                    ClearSelection();
                }
            }
        }

        private CTreeViewDragAndDropMode _DragAndDropMode;
        [DefaultValue(typeof(CTreeViewDragAndDropMode), "ReplaceReorder")]
        public CTreeViewDragAndDropMode DragAndDropMode
        {
            get { return _DragAndDropMode; }
            set
            {
                if (_DragAndDropMode != value)
                {
                    _DragAndDropMode = value;
                    Recalculate();
                }
            }
        }

        private int _indentDepth;
        /// <summary>
        /// Gets or sets the distance to indent each child tree node level.
        /// </summary>
        [DefaultValue(30)]
        public int IndentDepth                                   //To rename!
        {
            get { return _indentDepth; }
            set
            {
                if (_indentDepth != value)
                {
                    _indentDepth = value;
                    Recalculate();
                }
            }
        }

        private int _indentWidth;
        /// <summary>
        /// Gets or sets the minimal distance between child tree nodes.
        /// </summary>
        [DefaultValue(10)]
        public int IndentWidth                                   //To rename!
        {
            get { return _indentWidth; }
            set
            {
                if (_indentWidth != value)
                {
                    _indentWidth = value;
                    Recalculate();
                }
            }
        }

        private Pen _LinesPen;
        /// <summary>
        /// Gets or sets the Pen for drawing lines of this CTreeView
        /// </summary>
        [Browsable(false)]
        public Pen LinesPen
        {
            get { return _LinesPen; }
            set
            {
                _LinesPen.Dispose();
                _LinesPen = value;
                Refresh();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Collapses all the tree nodes.
        /// </summary>
        public void CollapseAll()
        {
            BeginUpdate();
            foreach (CTreeNode<T> node in Nodes) node.CollapseAll();
            EndUpdate();
        }

        /// <summary>
        /// Expands all the tree nodes.
        /// </summary>
        public void ExpandAll()
        {
            BeginUpdate();
            foreach (CTreeNode<T> node in Nodes) node.ExpandAll();
            EndUpdate();
        }

        /// <summary>
        /// Retrieves the number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.
        /// </summary>
        /// <param name="includeSubTrees">true  to count the TreeNode items that the subtrees contain; otherwise, false.</param>
        /// <returns>The number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.</returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            int result = Nodes.Count;
            if (includeSubTrees)
            {
                foreach (CTreeNode<T> node in Nodes) result += node.GetNodeCount(true);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the tree node that is at the specified point.
        /// </summary>
        /// <param name="pt">The Point to evaluate and retrieve the node from.</param>
        /// <returns>The CTreeNode<T> at the specified point, in client coordinates, or null if there is no node at that location.</returns>
        public CTreeNode<T> GetNodeAt(Point pt)
        {
            bool success = false;
            CTreeNode<T> nodeAtPoint = null;
            Nodes.TraverseNodes(node => node.Visible && !success, node =>
            {
                if (node.Bounds.Contains(pt))
                {
                    nodeAtPoint = node;
                    success = true;
                }
            });
            return nodeAtPoint;
        }

        /// <summary>
        /// Retrieves the tree node at the point with the specified coordinates.
        /// </summary>
        /// <param name="x">The X position to evaluate and retrieve the node from.</param>
        /// <param name="y">The Y position to evaluate and retrieve the node from.</param>
        /// <returns>The CTreeNode<T> at the specified location, in tree view (client) coordinates, or null if there is no node at that location.</returns>
        public CTreeNode<T> GetNodeAt(int x, int y)
        {
            return GetNodeAt(new Point(x,y));
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearSelection()
        {
            _selectedNodes.Clear();
            //Invalidate();
            //Update();
            Refresh();
        }

        /// <summary>
        /// Disables any redrawing of the CTreeView.
        /// </summary>
        public void BeginUpdate()
        {
            this.SuspendLayout();
            suspendUpdateCount++;
        }

        /// <summary>
        /// Enables the redrawing of the CTreeView.
        /// </summary>
        public void EndUpdate()
        {
            this.ResumeLayout(false);
            if (suspendUpdateCount > 0)
            {
                suspendUpdateCount--;
                if (suspendUpdateCount == 0) Recalculate();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after the tree node is collapsed.
        /// </summary>
        public event EventHandler<CTreeViewEventArgs> AfterCollapse;

        /// <summary>
        /// Occurs after the tree node is expanded.
        /// </summary>
        public event EventHandler<CTreeViewEventArgs> AfterExpand;

        /// <summary>
        /// Occurs after the tree node is selected.
        /// </summary>
        public event EventHandler<CTreeViewEventArgs> AfterSelect;

        /// <summary>
        /// Occurs when the user begins dragging a one or more nodes.
        /// </summary>
        public event EventHandler<DragNodesEventArgs> DragNodes;

        ///// <summary>
        ///// Occurs when a CTreeNodeControl of CTreeView<T> is drawn.
        ///// </summary>
        //public event EventHandler<CTreeViewEventArgs> ShowNodeControl;

        ///// <summary>
        ///// Occurs when the tree node is dragged.
        ///// </summary>
        //public event EventHandler<CTreeViewEventArgs> Drag;

        ///// <summary>
        ///// Occurs when the tree node is dropped.
        ///// </summary>
        //public event EventHandler<CTreeViewEventArgs> Drop;
        #endregion
    }
}