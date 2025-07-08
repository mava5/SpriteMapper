
namespace SpriteMapper.Actions.Global
{
    public class Undo : Action, IShort
    {
        public bool Do() { App.Project.History.Undo(); return true; }
    }
}
