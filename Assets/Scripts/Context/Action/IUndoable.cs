
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Interface used to create an <see cref="Action"/>, that can be undone and redone.
    /// <br/>   An undoable action gets saved to the action history stack.
    /// </summary>
    public interface IUndoable
    {
        public void Undo();
        public void Redo();
    }
}
