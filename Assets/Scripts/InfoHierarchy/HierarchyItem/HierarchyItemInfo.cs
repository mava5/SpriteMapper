
using System;
using System.Reflection;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains both shared and unique information for each copy of a <see cref="HierarchyItem"/>.
    /// <br/>   Shared information is gotten from the <see cref="HierarchyItemSettings"/>.
    /// </summary>
    public abstract class HierarchyItemInfo<T> where T : HierarchyItem
    {
        public readonly string Name;
        public readonly string Description;


        public HierarchyItemInfo(string name = "", string description = "")
        {
            if (string.IsNullOrEmpty(description))
            {
                Description = GetSettings()?.DefaultDescription;
            }
            else
            {
                Description = description;
            }
        }


        public HierarchyItemSettings GetSettings()
        {
            return typeof(T).GetCustomAttribute<HierarchyItemSettings>();
        }

        /// <summary> Creates a new hierarchy item with this info inputed to it. </summary>
        public HierarchyItem CreateItem()
        {
            HierarchyItem item = Activator.CreateInstance(typeof(T)) as HierarchyItem;
            
            item.Initialize(this as HierarchyItemInfo<HierarchyItem>);

            return item;
        }
    }
}
