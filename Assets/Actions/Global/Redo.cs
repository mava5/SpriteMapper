
namespace SpriteMapper.Actions
{
    [UserExecutable(typeof(Context.Global))]
    public class Redo : Action, IUserExecutable
    {
        public Redo() { App.Action.History.Redo(); }
    }
}
