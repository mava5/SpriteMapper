
namespace SpriteMapper.Actions
{
    [UserExecutable(typeof(Context.Global))]
    public class Undo : Action
    {
        public Undo() { App.Project.Action.History.Undo(); }
    }
}
