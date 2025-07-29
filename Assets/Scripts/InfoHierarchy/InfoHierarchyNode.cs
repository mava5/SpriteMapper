
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
        public string Path { get; private set; }

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

        public void SetParentRecursive(InfoHierarchyNode parent)
        {
            if (Parent != null) { return; }


            Parent = parent;

            Path = Parent?.Path ?? "";

            foreach (InfoHierarchyNode child in GetChildrenUnsorted())
            {
                child.SetParentRecursive(this);
            }
        }

        public List<InfoHierarchyNode> GetChildrenUnsorted()
        {
            return SortedChildren.Values.SelectMany(i => i).Distinct().ToList();
        }


        /// <summary>
        /// <br/>   Tells if this context can access given ancestor context.
        /// <br/>   Ancestor contexts cannot be seen through detachments.
        /// <br/>   This applies to global contexts as well.
        /// </summary>
        public bool CanAccess(InfoHierarchyNode item)
        {
            return
                Path == item.Path ||
                item.Path.StartsWith("Global.") && !Path.Contains("/") ||
                Path.StartsWith(item.Path) && Path.LastIndexOf('/', item.Path.Length) == -1;
        }

        /// <summary> Tells if this item shares path with given item. </summary>
        public bool IsUnder(InfoHierarchyNode item)
        {
            return Path.StartsWith(item.Path);
        }

        #endregion Public Methods
    }
}
