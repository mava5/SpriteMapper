
/// <summary>
/// <br/>   Interface used to create undoable and redoable actions.
/// <br/>   An undoable action gets saved to the action history stack.
/// </summary>
public interface IUndoable
{
    void Undo();
    void Redo();
}
