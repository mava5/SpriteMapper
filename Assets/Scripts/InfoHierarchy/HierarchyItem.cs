
using System;
using System.Linq;
using System.Collections.Generic;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Used to create a nested structure for the <see cref="InfoHierarchy"/>.
    /// <br/>   Stores hierarchy item's information and its position in hierarchy.
    /// </summary>
    public class InfoHierarchyItem
    {
        public string Path { get; private set; }
        public string FullName { get; private set; }
        public readonly string Name;
        public readonly string Description;

        /// <summary> Detached hierarchy items cannot access their ancestor items. </summary>
        public readonly bool Detached;

        public InfoHierarchyItem Parent { get; private set; }

        /// <summary> Children are separated based on the type of a hierarchy item they are. </summary>
        public readonly Dictionary<Type, List<InfoHierarchyItem>> SortedChildren;

        public List<InfoHierarchyItem> Children => SortedChildren.Values.SelectMany(i => i).Distinct().ToList();


        public InfoHierarchyItem(string name, bool detached, List<InfoHierarchyItem> children)
        {
            Name = name;
            Detached = detached;

            foreach (InfoHierarchyItem child in children)
            {
                SortedChildren.TryAdd(child.GetType(), new());
                SortedChildren[child.GetType()].Add(child);
            }
        }


        #region Public Methods ==================================================================== Public Methods

        public void SetParentRecursive(InfoHierarchyItem parent)
        {
            if (Parent != null) { return; }


            Parent = parent;

            Path = Parent?.Path ?? "";
            FullName = Path + (Detached ? "/" : ".") + Name;

            foreach (InfoHierarchyItem child in Children)
            {
                child.SetParentRecursive(this);
            }
        }

        /// <summary>
        /// <br/>   Tells if this context can access given ancestor context.
        /// <br/>   Ancestor contexts cannot be seen through detachments.
        /// <br/>   This applies to global contexts as well.
        /// </summary>
        public bool CanAccess(InfoHierarchyItem item)
        {
            return
                Path == item.Path ||
                item.Name == "Global" && !Path.Contains('/') ||
                Path.StartsWith(item.Path) && Path.LastIndexOf('/', item.Path.Length) == -1;
        }

        /// <summary> Tells if this item shares path with given item. </summary>
        public bool IsUnder(InfoHierarchyItem item)
        {
            return Path.StartsWith(item.Path);
        }

        #endregion Public Methods
    }
}