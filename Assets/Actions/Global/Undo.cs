
namespace SpriteMapper.Actions
{
    [UserExecutable(typeof(Context.Global))]
    public class Undo : Action, IUserExecutable
    {
        public Undo() { App.Action.History.Undo(); }
    }
}
