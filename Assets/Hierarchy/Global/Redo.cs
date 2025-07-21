
namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        public class Redo : Action, IShort
        {
            public bool Do() { App.Project.History.Redo(); return true; }
        }
    }
}
