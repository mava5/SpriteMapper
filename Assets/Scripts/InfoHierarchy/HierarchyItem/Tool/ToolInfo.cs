
namespace SpriteMapper
{
    /// <summary> Contains necessary information for a <see cref="Tool"/>. </summary>
    public class ToolInfo<T> : HierarchyItemInfo<T> where T : Tool
    {
        public ToolInfo(Shortcut defaultEquipShortcut, string nameOverwrite = "", string descriptionOverwrite = "")
            : base(nameOverwrite, descriptionOverwrite)
        {

        }
    }
}
