
namespace SpriteMapper.Actions
{
    [ActionContext(Context.Global)]
    public class Redo : Action, IUserExecutable
    {
        public Redo() { App.Action.History.Redo(); }
    }
}
