
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// <br/>   A static class, which handles the <see cref="Action"/> stack.
/// <br/>   Used to keep track of <see cref="IUndoable"/> <see cref="Action"/> history.
/// </summary>
public static class ActionHistory
{
    /// <summary>
    /// <br/>   Keeps track of actions that can be redone.
    /// <br/>   Top element of stack is the first redoable action.
    /// <br/>   Doing an action other than first redoable action will clear the stack.
    /// </summary>
    private static Stack<Action> redoHistory;

    /// <summary>
    /// <br/>   Keeps track of actions that can be undone.
    /// <br/>   Top element of stack is the latest undoable action.
    /// <br/>   Oldest actions are dynamically removed to keep the stack under maximum size.
    /// </summary>
    private static Stack<Action> undoHistory;



    #region Public Functions ====================================================================== Public Functions

    public static void SaveUndoStep(Action action)
    {

    }

    public static void Redo()
    {
        // Do first redoable action again, move it from redo stack to undo stack
        if (redoHistory.Count > 0)
        {
            redoHistory.Peek().Do();
            undoHistory.Push(redoHistory.Peek());
            redoHistory.Pop();
        }
    }

    public static void Undo()
    {
        // Revert changes done by latest action in undo stack and move it to redo stack.
        if (undoHistory.Count > 0)
        {
            undoHistory.Peek().Do();
            undoHistory.Pop();
        }
    }

    #endregion Public Functions
}
