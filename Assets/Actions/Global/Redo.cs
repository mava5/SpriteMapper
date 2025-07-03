
namespace SpriteMapper.Actions
{
    [UserExecutable(typeof(Context.Global))]
    public class Redo : Action
    {
        public Redo() { App.Project.Action.History.Redo(); }
    }
}
