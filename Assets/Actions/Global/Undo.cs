
namespace SpriteMapper.Actions
{
    [ActionContext(Context.Global)]
    public class Undo : Action, IUserExecutable
    {
        public Undo() { ActionHistory.Undo(); }
    }
}
