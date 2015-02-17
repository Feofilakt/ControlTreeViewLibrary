using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ControlTreeView
{
    public partial class CTreeView
    {
        [DllImport("user32.dll")]
        static extern int SendMessage(
               int hWnd,      // handle to destination window
               uint Msg,       // message
               long wParam,  // first message parameter
               long lParam   // second message parameter
               );

        /// <summary>
        /// 
        /// </summary>
        public struct DragTargetPositionClass
        {
            internal DragTargetPositionClass(CTreeNode nodeDirect, CTreeNode nodeBefore, CTreeNode nodeAfter)
            {
                _nodeDirect = nodeDirect;
                _nodeBefore = nodeBefore;
                _nodeAfter = nodeAfter;
            }

            /// <summary>
            /// Gets a value indicating whether drag destination nodes are not empty.
            /// </summary>
            public bool Enabled
            {
                get { return (_nodeDirect != null || _nodeBefore != null || _nodeAfter != null); }
            }

            private CTreeNode _nodeDirect;
            /// <summary>
            /// The direct node of drag target position.
            /// </summary>
            public CTreeNode NodeDirect
            {
                get { return _nodeDirect; }
            }

            private CTreeNode _nodeBefore;
            /// <summary>
            /// The upper node of drag target position.
            /// </summary>
            public CTreeNode NodeBefore
            {
                get { return _nodeBefore; }
            }

            private CTreeNode _nodeAfter;
            /// <summary>
            /// The lower node of drag target position.
            /// </summary>
            public CTreeNode NodeAfter
            {
                get { return _nodeAfter; }
            }
        }

        private Pen dragDropLinePen;
        private Point dragDropLinePoint1, dragDropLinePoint2;
        private Rectangle dragDropRectangle;
        private Timer scrollTimer;

        private void scrollTimer_Tick(object sender, EventArgs e)
        {
            if (scrollDown)SendMessage(this.Handle.ToInt32(), 277, 1, 0);
            else if (scrollUp) SendMessage(this.Handle.ToInt32(), 277, 0, 0);
            if (scrollRigh) SendMessage(this.Handle.ToInt32(), 276, 1, 0);
            else if (scrollLeft) SendMessage(this.Handle.ToInt32(), 276, 0, 0);
        }

        private bool scrollUp, scrollDown, scrollRigh, scrollLeft;
        /// <summary>
        /// Sets the directions in which need scroll.
        /// </summary>
        /// <param name="scrollUp">true if need scroll up, otherwise, false.</param>
        /// <param name="scrollDown">true if need scroll down, otherwise, false.</param>
        /// <param name="scrollRigh">true if need scroll right, otherwise, false.</param>
        /// <param name="scrollLeft">true if need scroll left, otherwise, false.</param>
        internal void SetScrollDirections(bool scrollUp, bool scrollDown, bool scrollRigh, bool scrollLeft)
        {
            if (scrollUp || scrollDown || scrollRigh || scrollLeft) scrollTimer.Enabled = true;
            else scrollTimer.Enabled = false;
            this.scrollUp = scrollUp;
            this.scrollDown = scrollDown;
            this.scrollRigh = scrollRigh;
            this.scrollLeft = scrollLeft;
        }

        private void updateDragTargetPosition()
        {
            if (DragTargetPosition.NodeDirect != null || DragTargetPosition.NodeBefore != null || DragTargetPosition.NodeAfter != null)
            {
                DragTargetPosition = new DragTargetPositionClass(null, null, null);
                dragDropLinePoint1 = Point.Empty;
                dragDropLinePoint2 = Point.Empty;
                dragDropRectangle = Rectangle.Empty;
                Refresh();
            }
        }

        private void updateDragTargetPosition(CTreeNode node)
        {
            if (DragTargetPosition.NodeDirect != node)
            {
                DragTargetPosition = new DragTargetPositionClass(node, null, null);
                dragDropRectangle = node.Bounds;
                dragDropRectangle.Inflate(2, 2);
                dragDropLinePoint1 = Point.Empty;
                dragDropLinePoint2 = Point.Empty;
                Refresh();
            }
        }

        private void updateDragTargetPosition(CTreeNode nodeBefore, CTreeNode nodeAfter)
        {
            if (DragTargetPosition.NodeBefore != nodeBefore || DragTargetPosition.NodeAfter != nodeAfter)
            {
                DragTargetPosition = new DragTargetPositionClass(null, nodeBefore, nodeAfter);
                if (nodeBefore == null)
                {
                    if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                    {
                        dragDropLinePoint1 = new Point(nodeAfter.BoundsSubtree.X - 2, nodeAfter.BoundsSubtree.Y);
                        dragDropLinePoint2 = new Point(nodeAfter.BoundsSubtree.X - 2, nodeAfter.BoundsSubtree.Bottom);
                    }
                    else
                    {
                        dragDropLinePoint1 = new Point(nodeAfter.BoundsSubtree.X, nodeAfter.BoundsSubtree.Y - 2);
                        dragDropLinePoint2 = new Point(nodeAfter.BoundsSubtree.Right, nodeAfter.BoundsSubtree.Y - 2);
                    }
                }
                else if (nodeAfter == null)
                {
                    if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                    {
                        dragDropLinePoint1 = new Point(nodeBefore.BoundsSubtree.Right + 2, nodeBefore.BoundsSubtree.Y);
                        dragDropLinePoint2 = new Point(nodeBefore.BoundsSubtree.Right + 2, nodeBefore.BoundsSubtree.Bottom);
                    }
                    else
                    {
                        dragDropLinePoint1 = new Point(nodeBefore.BoundsSubtree.X, nodeBefore.BoundsSubtree.Bottom + 2);
                        dragDropLinePoint2 = new Point(nodeBefore.BoundsSubtree.Right, nodeBefore.BoundsSubtree.Bottom + 2);
                    }
                }
                else
                {
                    if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                    {
                        int y1 = nodeBefore.BoundsSubtree.Y;
                        int y2 = Math.Max(nodeBefore.BoundsSubtree.Bottom, nodeAfter.BoundsSubtree.Bottom);
                        int x = nodeBefore.BoundsSubtree.Right + IndentWidth / 2;
                        dragDropLinePoint1 = new Point(x, y1);
                        dragDropLinePoint2 = new Point(x, y2);
                    }
                    else
                    {
                        int x1 = nodeBefore.BoundsSubtree.X;
                        int x2 = Math.Max(nodeBefore.BoundsSubtree.Right, nodeAfter.BoundsSubtree.Right);
                        int y = nodeBefore.BoundsSubtree.Bottom + IndentWidth / 2;
                        dragDropLinePoint1 = new Point(x1, y);
                        dragDropLinePoint2 = new Point(x2, y);
                    }
                }
                dragDropRectangle = Rectangle.Empty;
                Refresh();
            }
        }

        internal void ResetDragTargetPosition()
        {
            scrollTimer.Enabled = false;

            updateDragTargetPosition();
        }

        /// <summary>
        /// Sets the drag destination nodes according to specified cursor position.
        /// </summary>
        /// <param name="dragPosition">The position of mouse cursor during drag.</param>
        internal void SetDragTargetPosition(Point dragPosition)
        {
            CTreeNode destinationNode = null;
            CTreeNodeCollection destinationCollection = Nodes;
            Nodes.TraverseNodes(node => node.Visible && node.BoundsSubtree.Contains(dragPosition), node =>
            {
                destinationNode = node;
                destinationCollection = node.Nodes;
            });
            if (destinationNode != null && destinationNode.Bounds.Contains(dragPosition)) //Drag position within node
            {
                //Find drag position within node
                int delta, coordinate, firstBound, secondBound;
                if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                {
                    delta = destinationNode.Bounds.Width / 4;
                    coordinate = dragPosition.X;
                    firstBound = destinationNode.Bounds.Left;
                    secondBound = destinationNode.Bounds.Right;
                }
                else
                {
                    delta = destinationNode.Bounds.Height / 4;
                    coordinate = dragPosition.Y;
                    firstBound = destinationNode.Bounds.Top;
                    secondBound = destinationNode.Bounds.Bottom;
                }
                if (coordinate >= firstBound + delta && coordinate <= secondBound - delta)
                {
                    updateDragTargetPosition(destinationNode);
                    return;
                }
                else if (coordinate < firstBound + delta) //before
                {
                    updateDragTargetPosition(destinationNode.PrevNode, destinationNode);
                    return;
                }
                else if (coordinate > secondBound - delta) //after
                {
                    updateDragTargetPosition(destinationNode, destinationNode.NextNode);
                    return;
                }
            }
            else //Drag position out of the nodes
            {
                //Check drag position between two nodes
                CTreeNode upperNode = null, lowerNode = null;
                bool isBetween = false;
                for (int count = 0; count <= destinationCollection.Count - 2; count++)
                {
                    upperNode = destinationCollection[count];
                    lowerNode = destinationCollection[count + 1];
                    Point betweenLocation = Point.Empty;
                    Size betweenSize = Size.Empty;
                    if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                    {
                        betweenLocation = new Point(upperNode.BoundsSubtree.Right, upperNode.BoundsSubtree.Top);
                        betweenSize = new Size(lowerNode.BoundsSubtree.Left - upperNode.BoundsSubtree.Right, Math.Max(upperNode.BoundsSubtree.Height, lowerNode.BoundsSubtree.Height));
                    }
                    else
                    {
                        betweenLocation = new Point(upperNode.BoundsSubtree.Left, upperNode.BoundsSubtree.Bottom);
                        betweenSize = new Size(Math.Max(upperNode.BoundsSubtree.Width, lowerNode.BoundsSubtree.Width), lowerNode.BoundsSubtree.Top - upperNode.BoundsSubtree.Bottom);
                    }
                    Rectangle betweenRectangle = new Rectangle(betweenLocation, betweenSize);
                    if (betweenRectangle.Contains(dragPosition))
                    {
                        isBetween = true;
                        break;
                    }
                }
                if (isBetween) //Drag position between two nodes
                {
                    updateDragTargetPosition(upperNode, lowerNode);
                    return;
                }
                else if (destinationNode != null)
                {
                    Rectangle ownerBounds = destinationNode.Bounds;
                    bool isAbove, isBelow;
                    if (DrawStyle == CTreeViewDrawStyle.VerticalDiagram)
                    {
                        isAbove = (dragPosition.X <= ownerBounds.Left);
                        isBelow = (dragPosition.X >= ownerBounds.Right);
                    }
                    else
                    {
                        isAbove = (dragPosition.Y <= ownerBounds.Top);
                        isBelow = (dragPosition.Y >= ownerBounds.Bottom);
                    }
                    if (isAbove) //before
                    {
                        updateDragTargetPosition(destinationNode.PrevNode, destinationNode);
                        return;
                    }
                    else if (isBelow) //after
                    {
                        updateDragTargetPosition(destinationNode, destinationNode.NextNode);
                        return;
                    }
                }
            }
            updateDragTargetPosition();
        }

        /// <summary>
        /// Checking a valid of drop operation in current destination.
        /// </summary>
        /// <param name="sourceNodes">The source nodes of drag and drop operation.</param>
        /// <returns>true if drop of source nodes is allowed to current destination, otherwise, false.</returns>
        internal bool CheckValidDrop(List<CTreeNode> sourceNodes)
        {
            if (!DragTargetPosition.Enabled) return false;
            bool isValid = true;
            if (DragTargetPosition.NodeDirect != null)
            {
                if (DragAndDropMode == CTreeViewDragAndDropMode.Reorder) return false;
                else
                {
                    //Check that destination node is not descendant of source nodes
                    foreach (CTreeNode sourceNode in sourceNodes)
                    {
                        sourceNode.TraverseNodes(node => isValid, node =>
                        {
                            if (node == DragTargetPosition.NodeDirect) isValid = false;
                        });
                        if (!isValid) return false;
                    }
                }
            }
            else if (DragTargetPosition.NodeBefore != null || DragTargetPosition.NodeAfter != null)
            {
                //Check that source nodes are not moved relative themselves
                if (sourceNodes.Contains(DragTargetPosition.NodeBefore) && sourceNodes.Contains(DragTargetPosition.NodeAfter)) return false;
                if (sourceNodes.Contains(DragTargetPosition.NodeBefore) && DragTargetPosition.NodeAfter == null) return false;
                if (sourceNodes.Contains(DragTargetPosition.NodeAfter) && DragTargetPosition.NodeBefore == null) return false;
                if (DragAndDropMode == CTreeViewDragAndDropMode.Reorder)
                {
                    //Check that source and destination nodes have same parent
                    if (DragTargetPosition.NodeBefore != null && DragTargetPosition.NodeBefore.Parent != sourceNodes[0].Parent) return false;
                    if (DragTargetPosition.NodeAfter != null && DragTargetPosition.NodeAfter.Parent != sourceNodes[0].Parent) return false;
                }
                else
                {
                    //Check that destination nodes is not descendants of source nodes
                    foreach (CTreeNode sourceNode in sourceNodes)
                    {
                        sourceNode.Nodes.TraverseNodes(node => isValid, node =>
                        {
                            if (DragTargetPosition.NodeBefore == node || DragTargetPosition.NodeAfter == node) isValid = false;
                        });
                        if (!isValid) return false;
                    }
                }
            }
            return true;
        }
    }
}
