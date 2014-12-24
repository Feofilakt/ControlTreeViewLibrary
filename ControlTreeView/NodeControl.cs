using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using System.Collections.Generic;

namespace ControlTreeView
{
    /// <summary>
    /// Class that inherits from UserControl and implements specific selection logic and DragAndDrop logic.
    /// </summary>
    public class NodeControl : UserControl, INodeControl
    {
        public NodeControl()
        {
            DoubleBuffered = true;
        }

        //public event EventHandler UserSelect;
        //public event EventHandler UserMultiSelect;
        //public event EventHandler UserDrag;
        public CTreeNode OwnerNode { get; set; }

        /// <summary>
        /// it's experimental property for changing control's position relativly lines
        /// </summary>
        public virtual Rectangle Area
        {
            get { return new Rectangle(Point.Empty,Size); }
        }
        
        private Point mouseDownPosition;
        private bool unselectAfterMouseUp, unselectOtherAfterMouseUp; //Flags that indicates what need to do on MouseUp

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //Set selected nodes depends on selection mode
            unselectAfterMouseUp =unselectOtherAfterMouseUp= false;
            if (OwnerNode.OwnerCTreeView.SelectionMode != CTreeViewSelectionMode.None)
            {
                if (((Control.ModifierKeys & Keys.Control) == Keys.Control)&&
                    (OwnerNode.OwnerCTreeView.SelectionMode == CTreeViewSelectionMode.Multi ||
                    (OwnerNode.OwnerCTreeView.SelectionMode == CTreeViewSelectionMode.MultiSameParent &&
                    (OwnerNode.OwnerCTreeView.SelectedNodes.Count == 0 || OwnerNode.OwnerCTreeView.SelectedNodes[0].ParentNode == OwnerNode.ParentNode))))
                {
                    if (!OwnerNode.IsSelected) OwnerNode.IsSelected = true;
                    else unselectAfterMouseUp = true;
                    //UserMultiSelect(this, new EventArgs());
                }
                else
                {
                    if (!OwnerNode.IsSelected)
                    {
                        OwnerNode.OwnerCTreeView.ClearSelection();
                        OwnerNode.IsSelected = true;
                    }
                    else unselectOtherAfterMouseUp = true;
                    //UserSelect(this, new EventArgs());
                }
            }
            //Set handlers that handles start or not start dragging
            //mouseDownPosition = e.Location;
            mouseDownPosition = this.OwnerNode.OwnerCTreeView.PointToClient(Cursor.Position);
            this.MouseUp += new MouseEventHandler(NotDragging);
            this.MouseMove += new MouseEventHandler(StartDragging);

            base.OnMouseDown(e);
        }

        //Start dragging if mouse was moved
        private void StartDragging(object sender, MouseEventArgs e)
        {
            Point movePoint = this.OwnerNode.OwnerCTreeView.PointToClient(Cursor.Position);
            if (Math.Abs(mouseDownPosition.X - movePoint.X) + Math.Abs(mouseDownPosition.Y - movePoint.Y) > 5)
            //if (Math.Abs(mouseDownPosition.X - e.Location.X) + Math.Abs(mouseDownPosition.Y - e.Location.Y)>5)
            {
                this.MouseUp -= NotDragging;
                this.MouseMove -= StartDragging;

                OwnerNode.Drag();
                //UserDrag(this, new EventArgs());
            }
        }

        //Do not start dragging if mouse was up
        private void NotDragging(object sender, MouseEventArgs e)
        {
            this.MouseMove -= StartDragging;
            this.MouseUp -= NotDragging;
            if (unselectAfterMouseUp)
            {
                OwnerNode.IsSelected = false;
                //UserMultiSelect(this, new EventArgs());
            }
            if (unselectOtherAfterMouseUp)
            {
                List<CTreeNode> nodesToUnselect = new List<CTreeNode>(OwnerNode.OwnerCTreeView.SelectedNodes);
                nodesToUnselect.Remove(OwnerNode);
                foreach (CTreeNode node in nodesToUnselect) node.IsSelected = false;
                //UserMultiSelect(this, new EventArgs());
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NodeControl
            // 
            this.Name = "NodeControl";
            this.ResumeLayout(false);

        }
    }
}