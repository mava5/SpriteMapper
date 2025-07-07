
namespace SpriteMapper.Actions.Global
{
    public class OpenContextMenu : Action, IShort
    {
        public bool Do() { App.Project.FocusedPanel.OpenContextMenu(); return true; }
    }
}
