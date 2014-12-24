using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections.Generic;

namespace CTreeView
{
    [Designer(typeof(CTreeViewDesigner))]
    public partial class CTreeView<T> : ScrollableControl, INodeContainer
    {
        #region Properties
        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        /// <value>The default Size of the control.</value>
        protected override Size DefaultSize
        {
            get { return new Size(100, 100); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the Collapse event.
        /// </summary>
        /// <param name="e">A CTreeViewEventArgs that contains the event data.</param>
        protected internal virtual void OnAfterCollapse(CTreeViewEventArgs<T> e)
        {
            Recalculate();
            if (AfterCollapse != null) AfterCollapse(this, e);
        }

        /// <summary>
        /// Raises the Expand event.
        /// </summary>
        /// <param name="e">A CTreeViewEventArgs that contains the event data</param>
        protected internal virtual void OnAfterExpand(CTreeViewEventArgs e)
        {
            Recalculate();
            if (AfterExpand != null) AfterExpand(this, e);
        }

        /// <summary>
        /// Raises the Select event.
        /// </summary>
        /// <param name="e">A CTreeViewEventArgs that contains the event data</param>
        protected internal virtual void OnAfterSelect(CTreeViewEventArgs e)
        {
            //ParentCTreeView.Invalidate();
            //ParentCTreeView.Update();
            ParentCTreeView.Refresh();
            if (AfterSelect != null) AfterSelect(this, e);
        }

        /// <summary>
        /// Raises the ItemDrag event.
        /// </summary>
        /// <param name="e">An ItemDragEventArgs that contains the event data.</param>
        protected internal virtual void OnDragNodes(DragNodesEventArgs e)
        {
            if (DragAndDropMode != CTreeViewDragAndDropMode.Nothing)
            {
                // Move the dragged node when the left mouse button is used.
                if (e.Button == MouseButtons.Left)
                {
                    DoDragDrop(e.Nodes, DragDropEffects.Move);
                    if (DragNodes != null) DragNodes(this, e);
                }
                //// Copy the dragged node when the right mouse button is used.
                //else if (e.Button == MouseButtons.Right && DragAndDropMode == CTreeViewDragAndDropMode.CopyReplaceReorder)
                //{
                //    DoDragDrop(e.Nodes, DragDropEffects.Copy);
                //    if (DragNodes != null) DragNodes(this, e);
                //}
            }
        }
        
        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="drgevent">A DragEventArgs that contains the event data.</param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(List<CTreeNode>)) && DragAndDropMode != CTreeViewDragAndDropMode.Nothing)
            {
                try
                {
                    List<CTreeNode> sourceNodes = drgevent.Data.GetData(typeof(List<CTreeNode>)) as List<CTreeNode>;
                    Point dragPosition = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                    dragDestination.SetScroll(
                        VScroll && dragPosition.Y <20,
                        VScroll && dragPosition.Y > ClientSize.Height - 20,
                        HScroll && dragPosition.X > ClientSize.Width - 20,
                        HScroll && dragPosition.X < 20);
                    dragPosition.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
                    dragDestination.Set(dragPosition);
                    if (/*dragDestination.Enabled &&*/ dragDestination.CheckValidDrop(sourceNodes)) drgevent.Effect = drgevent.AllowedEffect;
                    else drgevent.Effect = DragDropEffects.None;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + '\n' + "Исключение в " + ex.Source + '\n' + ex.StackTrace, "Error"); //for debugging
                }
            }
            base.OnDragOver(drgevent);
        }

        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        /// <param name="drgevent">A DragEventArgs that contains the event data.</param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(List<CTreeNode>)) && DragAndDropMode != CTreeViewDragAndDropMode.Nothing)
            {
                drgevent.Effect = drgevent.AllowedEffect;
            }
            base.OnDragEnter(drgevent);
        }

        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnDragLeave(EventArgs e)
        {
            if (DragAndDropMode != CTreeViewDragAndDropMode.Nothing /*&& dragDestination.Enabled*/) dragDestination.Reset();
            base.OnDragLeave(e);
        }

        /// <summary>
        /// Raises the DragDrop event.
        /// </summary>
        /// <param name="drgevent">A DragEventArgs that contains the event data.</param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(List<CTreeNode>)) && DragAndDropMode != CTreeViewDragAndDropMode.Nothing)
            {
                try
                {
                    List<CTreeNode> sourceNodes = drgevent.Data.GetData(typeof(List<CTreeNode>)) as List<CTreeNode>;
                    Point dragPosition = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                    dragPosition.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
                    dragDestination.Set(dragPosition);
                    if (dragDestination.CheckValidDrop(sourceNodes))
                    {
                        BeginUpdate();
                        //if (drgevent.AllowedEffect == DragDropEffects.Copy)
                        //{
                        //    List<CTreeNode> copyNodes = new List<CTreeNode>(sourceNodes.Count);
                        //    foreach (CTreeNode<T> sourceNode in sourceNodes) copyNodes.Add((CTreeNode)sourceNode.Clone());
                        //    if (dragDestination.Node != null) dragDestination.Node.Nodes.AddRange(copyNodes.ToArray());
                        //    else if (dragDestination.NodeBefore != null)
                        //    {
                        //        int index = dragDestination.NodeBefore.Index + 1;
                        //        foreach (CTreeNode<T> copyNode in copyNodes) dragDestination.NodeBefore.OwnerCollection.Insert(index++, copyNode);
                        //    }
                        //    else if (dragDestination.NodeAfter != null)
                        //    {
                        //        int index = dragDestination.NodeAfter.Index;
                        //        foreach (CTreeNode<T> copyNode in copyNodes) dragDestination.NodeAfter.OwnerCollection.Insert(index++, copyNode);
                        //    }
                        //}
                        //else
                        //{
                            foreach (CTreeNode<T> sourceNode in sourceNodes) sourceNode.OwnerCollection.Remove(sourceNode);
                            if (dragDestination.Node != null) dragDestination.Node.Nodes.AddRange(sourceNodes.ToArray());
                            else if (dragDestination.NodeBefore != null && !sourceNodes.Contains(dragDestination.NodeBefore))
                            {
                                int index = dragDestination.NodeBefore.Index + 1;
                                foreach (CTreeNode<T> sourceNode in sourceNodes) dragDestination.NodeBefore.OwnerCollection.Insert(index++, sourceNode);
                            }
                            else if (dragDestination.NodeAfter != null && !sourceNodes.Contains(dragDestination.NodeAfter))
                            {
                                int index = dragDestination.NodeAfter.Index;
                                foreach (CTreeNode<T> sourceNode in sourceNodes) dragDestination.NodeAfter.OwnerCollection.Insert(index++, sourceNode);
                            }
                        //}
                        EndUpdate();
                    }
                    dragDestination.Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + '\n' + "Исключение в " + ex.Source + '\n' + ex.StackTrace, "Error"); //for debugging
                }
            }
            base.OnDragDrop(drgevent);
        }

        /// <summary>
        /// Raises the Scroll event.
        /// </summary>
        /// <param name="se">A ScrollEventArgs that contains the event data.</param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            //Recalculate();
        }

        /// <summary>
        /// Raises the MouseWheel event.
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            //Recalculate();
        }

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ShowPlusMinus)
            {
                CTreeNode<T> toggleNode = null;
                this.Nodes.TraverseNodes(node => node.Visible && node.Nodes.Count > 0, node =>
                {
                    Point cursorLocation = e.Location;
                    cursorLocation.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
                    if (node.PlusMinus != null && node.PlusMinus.IsUnderMouse(cursorLocation))
                    {
                        toggleNode = node;
                    }
                });
                ClearSelection();
                if (toggleNode != null)
                {
                    toggleNode.Toggle();
                    if (SelectionMode != CTreeViewSelectionMode.None) toggleNode.IsSelected = true;
                }
            }
            this.Focus();

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //if (!SuspendUpdate)
            //{
                e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

                //Paint lines
                if (ShowLines)
                {
                    this.Nodes.TraverseNodes(node => node.IsExpanded, node =>
                    {
                        if (node.Lines != null)
                        {
                            foreach (CTreeNode.Line line in node.Lines) e.Graphics.DrawLine(LinesPen, line.Point1, line.Point2);
                        }
                    });
                    if (rootLines != null)
                    {
                        foreach (CTreeNode.Line line in rootLines) e.Graphics.DrawLine(LinesPen, line.Point1, line.Point2);
                    }
                    //g.Dispose();
                }

                //Paint drag and drop destination animation.
                if (dragDestination.Enabled)
                {
                    dragDestination.DrawEffect(e.Graphics);
                }

                //Paint selection
                //if (true)
                //{
                foreach (CTreeNode<T> node in SelectedNodes)
                {
                    Rectangle selectionRectangle = node.Bounds;
                    selectionRectangle.Inflate(3, 3);
                    //if(!duringDragging) e.Graphics.FillRectangle(selectionBrush, selectionRectangle);
                    if (!dragDestination.Enabled) e.Graphics.FillRectangle(selectionBrush, selectionRectangle);
                    selectionRectangle.Width--; selectionRectangle.Height--;//костыль
                    e.Graphics.DrawRectangle(selectionPen, selectionRectangle);
                }
                //}

                //Paint PlusMinus buttons
                if (ShowPlusMinus)
                {
                    this.Nodes.TraverseNodes(node => node.Visible && node.Nodes.Count > 0, node =>
                    {
                        if (node.PlusMinus != null)
                        {
                            if (node.IsExpanded) e.Graphics.DrawImage(PlusMinus.Minus, node.PlusMinus.Location);
                            else e.Graphics.DrawImage(PlusMinus.Plus, node.PlusMinus.Location);
                        }
                    });
                }

                //Test bounds
                //this.Nodes.TraverseNodes(node => node.Visible, node =>
                //{
                //    e.Graphics.DrawRectangle(new Pen(Color.Silver, 1.0F), node.BoundsSubtree);
                //});
            //}

            base.OnPaint(e);
        }
        #endregion
    }
}