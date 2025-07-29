
namespace SpriteMapper
{
    /// <summary> An object created from a <see cref="HierarchyItemInfo"/> within <see cref="InfoHierarchy"/>. </summary>
    public abstract class HierarchyItem
    {
        public HierarchyItemInfo<HierarchyItem> Info { get; private set; }


        public void Initialize(HierarchyItemInfo<HierarchyItem> info)
        {
            Info = info;
        }
    }
}
