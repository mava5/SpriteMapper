
/// <summary>
/// <br/>   Interface used to create actions, that can be undone and redone.
/// <br/>   An undoable action gets saved to the action history stack.
/// </summary>
public interface IUndoable
{
    public void Undo();
    public void Redo();

    /// <summary>
    /// <br/>   Returns the amount of memory taken by action in bytes.
    /// <br/>   
    /// <br/>   Only big data structures are take to account so actual size is higher.
    /// <br/>   Right now only returns size of chosen variables with sizeof().
    /// </summary>
    public int GetSize();
}
