
namespace SpriteMapper
{
    /// <summary> Contains necessary information for a <see cref="Tool"/>. </summary>
    public class PanelInfo<T> : HierarchyItemInfo<T> where T : Panel
    {
        public PanelInfo(string nameOverwrite = "", string descriptionOverwrite = "")
            : base(nameOverwrite, descriptionOverwrite)
        {

        }
    }
}
