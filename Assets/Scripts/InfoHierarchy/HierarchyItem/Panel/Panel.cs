
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A focusable rectangular area with a given actions and tools.
    /// <br/>   Inherits ancestor panels' actions and tools as well.
    /// </summary>
    public abstract class Panel : HierarchyItem
    {
        public bool IsToolEquipped => Tool != null;

        /// <summary> Currently equipped <see cref="SpriteMapper.Tool"/>. </summary>
        public Tool Tool { get; internal set; } = null;

        /// <summary> Panel's unique context menu generated based on its context. </summary>
        public ContextMenu ContextMenu { get; internal set; } = null;


        public abstract void EquipTool<T>() where T : Tool;
    }
}
