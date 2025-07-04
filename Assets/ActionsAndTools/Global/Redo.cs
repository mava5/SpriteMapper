
namespace SpriteMapper.Actions
{
    [ActionUsedIn(typeof(Context.Global))]
    public class Redo : Action, IQuick
    {
        public bool Do() { App.Project.History.Redo(); return true; }
    }
}
