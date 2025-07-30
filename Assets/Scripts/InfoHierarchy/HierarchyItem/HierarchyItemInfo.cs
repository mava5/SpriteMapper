
using System;
using System.Reflection;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains both shared and unique information for each copy of a <see cref="HierarchyItem"/>.
    /// <br/>   Shared information is gotten from the <see cref="HierarchyItemSettings"/>.
    /// </summary>
    public class HierarchyItemInfo<T> where T : HierarchyItem
    {
        public readonly string Name;
        public readonly string Description;
        public readonly HierarchyItemSettings BaseSettings;


        public HierarchyItemInfo(string name, string description)
        {
            BaseSettings = typeof(T).GetCustomAttribute<HierarchyItemSettings>();

            if (string.IsNullOrEmpty(name))
            {
                Name = typeof(T).Name;
            }
            else
            {
                Name = name;
            }

            if (string.IsNullOrEmpty(description))
            {
                Description = BaseSettings?.DefaultDescription ?? "";
            }
            else
            {
                Description = description;
            }
        }


        /// <summary> Creates a new hierarchy item with this info inputed to it. </summary>
        public HierarchyItem CreateItem()
        {
            HierarchyItem item = Activator.CreateInstance(typeof(T)) as HierarchyItem;
            
            item.SetInfo(this as HierarchyItemInfo<HierarchyItem>);

            return item;
        }
    }
}
