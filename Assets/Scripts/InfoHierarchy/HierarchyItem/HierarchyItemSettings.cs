
using System;


namespace SpriteMapper
{
    /// <summary> Contains shared information for a <see cref="HierarchyItem"/>. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class HierarchyItemSettings : Attribute
    {
        /// <summary> Description used if not overwritten in <see cref="InfoHierarchy"/>. </summary>
        public string DefaultDescription;


        public HierarchyItemSettings(string defaultDescription)
        {
            DefaultDescription = defaultDescription;
        }
    }
}
