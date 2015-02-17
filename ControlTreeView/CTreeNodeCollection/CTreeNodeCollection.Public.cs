using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlTreeView
{
    /// <summary>
    /// Represents a collection of CTreeNode objects.
    /// </summary>
    public partial class CTreeNodeCollection : Collection<CTreeNode>
    {
        /// <summary>
        /// Apply action to each node in CTreeNodeCollection and recursively to child nodes.
        /// </summary>
        /// <param name="action">Action will be applied to the nodes.</param>
        public void TraverseNodes(Action<CTreeNode> action)
        {
            foreach (CTreeNode childNode in this) childNode.TraverseNodes(action);
        }

        /// <summary>
        /// Apply action to each node in CTreeNodeCollection and recursively to child nodes until the condition is true.
        /// </summary>
        /// <param name="condition">Condition that must be satisfied node.</param>
        /// <param name="action">Action will be applied to the nodes.</param>
        public void TraverseNodes(Func<CTreeNode, bool> condition, Action<CTreeNode> action)
        {
            foreach (CTreeNode childNode in this) childNode.TraverseNodes(condition, action);
        }

        /// <summary>
        /// Adds an array of previously created tree nodes to the collection.
        /// </summary>
        /// <param name="nodes">An array of CTreeNode objects representing the tree nodes to add to the collection.</param>
        /// <exception cref="ArgumentNullException">Nodes is null.</exception>
        public virtual void AddRange(CTreeNode[] nodes)
        {
            if (nodes == null) throw new ArgumentNullException("Nodes is null.");
            BeginUpdateCTreeView();
            foreach (CTreeNode node in nodes) Add(node);
            EndUpdateCTreeView();
        }

        /// <summary>
        /// Inserts an array of previously created tree nodes to the collection at the specific position.
        /// </summary>
        /// <param name="index">The zero-based index at which nodes should be inserted.</param>
        /// <param name="nodes">An array of CTreeNode objects representing the tree nodes to add to the collection.</param>
        public virtual void InsertRange(int index, CTreeNode[] nodes)
        {
            if (nodes == null) throw new ArgumentNullException("Nodes is null.");
            BeginUpdateCTreeView();
            foreach (CTreeNode node in nodes) Insert(index++, node);
            EndUpdateCTreeView();
        }

        //public virtual void RemoveRange(CTreeNode[] nodes)
        //{
        //    if (nodes == null) throw new ArgumentNullException("Nodes is null.");
        //    BeginUpdateCTreeView();
        //    foreach (CTreeNode node in nodes) Remove(node);
        //    EndUpdateCTreeView();
        //}

        /// <summary>
        /// Gets the tree node with the specified key from the collection.
        /// </summary>
        /// <param name="key">The name of the CTreeNode to retrieve from the collection.</param>
        /// <returns>The CTreeNode with the specified key.</returns>
        public virtual CTreeNode this[string key]
        {
            get
            {
                foreach (CTreeNode node in this)
                {
                    if (node.Name == key) return node;
                }
                return null;
            }
        }

        /// <summary>
        /// Finds the tree nodes with specified key, optionally searching subnodes.
        /// </summary>
        /// <param name="key">The name of the tree node to search for.</param>
        /// <param name="searchAllChildren">true  to search child nodes of tree nodes; otherwise, false.</param>
        /// <returns>An array of CTreeNode objects whose Name property matches the specified key.</returns>
        public CTreeNode[] Find(string key, bool searchAllChildren)
        {
            if (key == null || key == "") return new CTreeNode[0];
            List<CTreeNode> foundNodes = new List<CTreeNode>();
            if (searchAllChildren)
            {
                TraverseNodes(node => { if (node.Name == key) foundNodes.Add(node); });
            }
            else
            {
                foreach (CTreeNode node in this) if (node.Name == key) foundNodes.Add(node);
            }
            return foundNodes.ToArray();
        }

        /// <summary>
        /// Removes the tree node with the specified key from the collection.
        /// </summary>
        /// <param name="key">The name of the tree node to remove from the collection.</param>
        public virtual void RemoveByKey(string key)
        {
            foreach (CTreeNode node in Find(key, false)) Remove(node);
            // or
            // Remove(this[key]);
            // ?
        }

        /// <summary>
        /// Returns the index of the first occurrence of a tree node with the specified key.
        /// </summary>
        /// <param name="key">The name of the tree node to search for.</param>
        /// <returns>The zero-based index of the first occurrence of a tree node with the specified key, if found; otherwise, -1.</returns>
        public virtual int IndexOfKey(string key)
        {
            return IndexOf(this[key]);
        }

        /// <summary>
        /// Determines whether the collection contains a tree node with the specified key.
        /// </summary>
        /// <param name="key">The name of the CTreeNode to search for.</param>
        /// <returns>true  to indicate the collection contains a CTreeNode with the specified key; otherwise, false.</returns>
        public virtual bool ContainsKey(string key)
        {
            foreach (CTreeNode node in this)
                {
                    if (node.Name == key) return true;
                }
            return false;
        }
    }
}
