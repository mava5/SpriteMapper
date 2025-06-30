
namespace SpriteMapper.Actions
{
    [ActionContext(Context.Global)]
    public class Redo : Action, IUserExecutable
    {
        public Redo() { ActionHistory.Redo(); }
    }
}
