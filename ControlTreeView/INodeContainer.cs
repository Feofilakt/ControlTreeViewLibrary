using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlTreeView
{
    /// <summary>
    /// The container for CTreeNodeCollection.
    /// </summary>
    public interface INodeContainer
    {
        #region Properties

        /// <summary>
        /// Gets the collection of tree nodes that are assigned to this INodeContainer.
        /// </summary>
        CTreeNodeCollection Nodes { get; }

        /// <summary>
        /// Gets the first child tree node in the tree node collection.
        /// </summary>
        CTreeNode FirstNode { get; }

        /// <summary>
        /// Gets the last child tree node in the tree node collection.
        /// </summary>
        CTreeNode LastNode { get; }

        /// <summary>
        /// Gets the tree view that the CTreeNodeCollection of this INodeContainer is assigned to.
        /// </summary>
        CTreeView OwnerCTreeView { get; }

        /// <summary>
        /// Gets the bounds of this INodeContainer includes all child tree nodes.
        /// </summary>
        Rectangle BoundsSubtree { get; }

        /// <summary>
        /// Gets or sets the name of the INodeContainer.
        /// </summary>
        string Name { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Expands all the tree nodes of this INodeContainer.
        /// </summary>
        void ExpandAll();

        /// <summary>
        /// Collapses all the tree nodes of this INodeContainer.
        /// </summary>
        void CollapseAll();

        /// <summary>
        /// Returns the number of child tree nodes.
        /// </summary>
        /// <param name="includeSubTrees">true  if the resulting count includes all tree nodes indirectly rooted at the tree node collection; otherwise, false.</param>
        /// <returns>The number of child tree nodes assigned to the CTreeNodeCollection.</returns>
        int GetNodeCount(bool includeSubTrees);

        #endregion
    }
}
