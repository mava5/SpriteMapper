
namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        public class Undo : Action, IShort
        {
            public bool Do() { App.Project.History.Undo(); return true; }
        }
    }
}
