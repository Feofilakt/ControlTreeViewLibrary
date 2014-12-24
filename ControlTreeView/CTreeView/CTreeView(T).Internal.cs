using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CTreeView
{
    [Designer(typeof(CTreeViewDesigner))]
    public partial class CTreeView<T> : ScrollableControl,INodeContainer
    {
        /// <summary>
        /// The union of all child nodes bounds.
        /// </summary>
        public Rectangle BoundsSubtree { get; internal set; }

        private int suspendUpdateCount;
        /// <summary>
        /// 
        /// </summary>
        internal bool SuspendUpdate
        {
            get
            {
                if (suspendUpdateCount > 0) return true;
                else return false;
            }
        }

        /// <summary>
        /// The list of lines for the CTreeView.
        /// </summary>
        private List<CTreeNode.Line> rootLines;

        private DragDestination dragDestination;

        internal void Recalculate()
        {
            if (!SuspendUpdate)
            {
                Func<CTreeNode, Point> plusMinusCalc = null;
                Func<CTreeNode, CTreeNode.Line> parentLineCalc = null;
                Func<CTreeNodeCollection<T>, CTreeNode.Line> commonLineCalc = null;
                Func<CTreeNode, CTreeNode.Line> childLineCalc = null;
                const int endLineIndent = 2;
                bool showRootPlusMinus = true;

                //Calculate Visible (may be optimized)
                foreach (CTreeNode<T> rootNode in Nodes)
                {
                    rootNode.Nodes.TraverseNodes(node => { node.Visible = false; });
                    rootNode.Nodes.TraverseNodes(node => node.Parent.IsExpanded, node =>
                    {
                        node.Visible = true;
                    });
                }

                switch (DrawStyle)
                {
                    case CTreeViewDrawStyle.LinearTree:
                        showRootPlusMinus = ShowRootLines;
                        Point startLocation = new Point(Padding.Left + 3, Padding.Top + 3);
                        if (ShowRootLines) startLocation.X += IndentDepth;
                        foreach (CTreeNode<T> node in Nodes) startLocation.Y = node.NextLocation(startLocation).Y;
                        plusMinusCalc = new Func<CTreeNode, Point>(eachNode =>
                            new Point(eachNode.Location.X - IndentDepth + 5, eachNode.Location.Y + eachNode.Control.Height / 2));
                        parentLineCalc = new Func<CTreeNode, CTreeNode.Line>(parent => new CTreeNode.Line(
                            new Point(parent.Location.X + 5, parent.Location.Y + parent.Control.Height + endLineIndent),
                            new Point(parent.Location.X + 5, parent.FirstNode.Location.Y + parent.FirstNode.Control.Height / 2)));
                        commonLineCalc = new Func<CTreeNodeCollection<T>, CTreeNode.Line>(nodes => new CTreeNode.Line(
                            new Point(nodes[0].Location.X - IndentDepth + 5, nodes[0].Location.Y + nodes[0].Control.Height / 2),
                            new Point(nodes[0].Location.X - IndentDepth + 5, nodes[nodes.Count - 1].Location.Y + nodes[nodes.Count - 1].Control.Height / 2)));
                        childLineCalc = new Func<CTreeNode, CTreeNode.Line>(child => new CTreeNode.Line(
                            new Point(child.Location.X - IndentDepth + 5, child.Location.Y + child.Control.Height / 2),
                            new Point(child.Location.X - endLineIndent, child.Location.Y + child.Control.Height / 2)));
                        break;

                    case CTreeViewDrawStyle.HorizontalDiagram:
                        int startX = Padding.Left + 3;
                        int startYMax = Padding.Top + 3;
                        if (ShowRootLines) startX += IndentDepth;
                        foreach (CTreeNode<T> node in Nodes) startYMax = node.NextYMax(startX, startYMax);
                        plusMinusCalc = new Func<CTreeNode, Point>(eachNode =>
                            new Point(eachNode.Location.X + eachNode.Control.Width + PlusMinus.Size.Width / 2 + 2, eachNode.Location.Y + eachNode.Control.Height / 2));
                        parentLineCalc = new Func<CTreeNode, CTreeNode.Line>(parent => new CTreeNode.Line(
                            new Point(parent.Location.X + parent.Control.Width + endLineIndent, parent.Location.Y + parent.Control.Height / 2),
                            new Point(parent.Location.X + parent.Control.Width + IndentDepth / 2, parent.Location.Y + parent.Control.Height / 2)));
                        commonLineCalc = new Func<CTreeNodeCollection<T>, CTreeNode.Line>(nodes => new CTreeNode.Line(
                            new Point(nodes[0].Location.X - IndentDepth / 2, nodes[0].Location.Y + nodes[0].Control.Height / 2),
                            new Point(nodes[0].Location.X - IndentDepth / 2, nodes[nodes.Count - 1].Location.Y + nodes[nodes.Count - 1].Control.Height / 2)));
                        childLineCalc = new Func<CTreeNode, CTreeNode.Line>(child => new CTreeNode.Line(
                            new Point(child.Location.X - IndentDepth / 2, child.Location.Y + child.Control.Height / 2),
                            new Point(child.Location.X - endLineIndent, child.Location.Y + child.Control.Height / 2)));
                        break;

                    case CTreeViewDrawStyle.VerticalDiagram:
                        int startXMax = Padding.Left + 3;
                        int startY = Padding.Top + 3;
                        if (ShowRootLines) startY += IndentDepth;
                        foreach (CTreeNode<T> node in Nodes) startXMax = node.NextXMax(startXMax, startY);
                        plusMinusCalc = new Func<CTreeNode, Point>(eachNode =>
                            new Point(eachNode.Location.X + eachNode.Control.Width / 2, eachNode.Location.Y + eachNode.Control.Height + PlusMinus.Size.Width / 2 + 2));
                        parentLineCalc = new Func<CTreeNode, CTreeNode.Line>(parent => new CTreeNode.Line(
                            new Point(parent.Location.X + parent.Control.Width / 2, parent.Location.Y + parent.Control.Height + endLineIndent),
                            new Point(parent.Location.X + parent.Control.Width / 2, parent.Location.Y + parent.Control.Height + IndentDepth / 2)));
                        commonLineCalc = new Func<CTreeNodeCollection<T>, CTreeNode.Line>(nodes => new CTreeNode.Line(
                            new Point(nodes[0].Location.X + nodes[0].Control.Width / 2, nodes[0].Location.Y - IndentDepth / 2),
                            new Point(nodes[nodes.Count - 1].Location.X + nodes[nodes.Count - 1].Control.Width / 2, nodes[0].Location.Y - IndentDepth / 2)));
                        childLineCalc = new Func<CTreeNode, CTreeNode.Line>(child => new CTreeNode.Line(
                            new Point(child.Location.X + child.Control.Width / 2, child.Location.Y - IndentDepth / 2),
                            new Point(child.Location.X + child.Control.Width / 2, child.Location.Y - endLineIndent)));
                        break;
                }

                //Calculate PlusMinus
                if (ShowPlusMinus)
                {
                    foreach (CTreeNode<T> node in Nodes) node.CalculatePlusMinus(plusMinusCalc, showRootPlusMinus);
                }

                //Calculate Lines
                if (ShowLines)
                {
                    foreach (CTreeNode<T> node in Nodes) node.CalculateLines(parentLineCalc, commonLineCalc, childLineCalc);
                    if (ShowRootLines) rootLines = Nodes.GetLines(commonLineCalc, childLineCalc);
                    else rootLines = null;
                }

                //Calculate Bounds
                BoundsSubtree = Rectangle.Empty;
                foreach (CTreeNode<T> node in Nodes)
                {
                    node.CalculateBounds();
                    BoundsSubtree = Rectangle.Union(node.BoundsSubtree, BoundsSubtree);
                }

                //Locate controls
                this.SuspendLayout();
                this.Nodes.TraverseNodes(node =>
                {
                    node.Control.Visible = node.Visible;
                    //if (node.Control.Visible == true)
                    //{
                    Point controlLocation = node.Location;
                    controlLocation.Offset(AutoScrollPosition);
                    node.Control.Location = controlLocation;
                    //}
                });
                this.ResumeLayout(false);

                this.AutoScrollMinSize = new Size(BoundsSubtree.Width + Padding.Right, BoundsSubtree.Height + Padding.Bottom);
                //Invalidate();
                //Update();
                Refresh();
            }
        }
    }
}