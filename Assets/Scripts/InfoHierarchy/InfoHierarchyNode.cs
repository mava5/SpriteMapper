
using System;
using System.Linq;
using System.Collections.Generic;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Used to create a nested structure for the <see cref="InfoHierarchy"/>.
    /// <br/>   Stores context and both shared and unique information for a <see cref="HierarchyItem"/>.
    /// </summary>
    public class InfoHierarchyNode
    {
        /// <summary> The path (concatenation of ancestor items' names) leading up to the item. </summary>
        public string Context { get; private set; }

        public int TotalDetachments { get; private set; }

        public InfoHierarchyNode Parent { get; private set; }

        /// <summary> Detached hierarchy items cannot access their ancestor items. </summary>
        public readonly bool Detached;

        public readonly HierarchyItemInfo<HierarchyItem> Info;

        /// <summary> Children are separated based on the type of a hierarchy item they are. </summary>
        public readonly Dictionary<Type, List<InfoHierarchyNode>> SortedChildren;


        public InfoHierarchyNode(bool detached, HierarchyItemInfo<HierarchyItem> info, List<InfoHierarchyNode> children)
        {
            Detached = detached;
            Info = info;

            foreach (InfoHierarchyNode child in children)
            {
                SortedChildren.TryAdd(child.GetType(), new());
                SortedChildren[child.GetType()].Add(child);
            }
        }


        #region Public Methods ==================================================================== Public Methods

        public void SetParentRecursive(InfoHierarchyNode parent, int totalDetachments)
        {
            if (Parent != null) { return; }


            Parent = parent;

            if (Parent == null) { Context = ""; }
            else { Context = Parent.Context + (Parent.Detached ? "/" : ".") + Parent.Info.Name; }

            if (Detached) { totalDetachments++; }

            foreach (InfoHierarchyNode child in GetChildrenUnsorted())
            {
                child.SetParentRecursive(this, totalDetachments);
            }
        }

        public List<InfoHierarchyNode> GetChildrenUnsorted()
        {
            return SortedChildren.Values.SelectMany(i => i).Distinct().ToList();
        }

        /// <summary>
        /// <br/>   Tells if this node can access other ancestor node.
        /// <br/>   Ancestor nodes cannot be seen through detachments.
        /// <br/>   This applies to global contexts as well.
        /// </summary>
        public bool CanAccess(InfoHierarchyNode other)
        {
            return
                Context == other.Context ||
                other.Context.Contains(InfoHierarchy.GlobalContext) && TotalDetachments == 0 ||
                Context.StartsWith(other.Context) && TotalDetachments == other.TotalDetachments;
        }

        /// <summary> Tells if this node shares path with other node. </summary>
        public bool IsUnder(InfoHierarchyNode other)
        {
            return Context.StartsWith(other.Context);
        }

        #endregion Public Methods
    }
}
