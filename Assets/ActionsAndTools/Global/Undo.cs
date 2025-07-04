
namespace SpriteMapper.Actions
{
    [ActionUsedIn(typeof(Context.Global))]
    public class Undo : Action, IQuick
    {
        public bool Do() { App.Project.History.Undo(); return true; }
    }
}
