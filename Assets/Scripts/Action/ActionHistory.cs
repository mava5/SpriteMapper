
using System.Collections.Generic;
using System.Linq;
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
    /// <br/>   
    /// 
    /// TODO: Redo history
    /// 1. Doing an action other than first redoable action will clear the stack.
    /// 2. Doing any action will clear the stack.
    /// </summary>
    private static Stack<IUndoable> redoHistory = new();

    /// <summary>
    /// <br/>   Keeps track of actions that can be undone.
    /// <br/>   Top element of stack is the latest undoable action.
    /// <br/>   Oldest actions are removed to keep the stack under maximum size.
    /// </summary>
    private static Stack<IUndoable> undoHistory = new();


    #region Public Functions ====================================================================== Public Functions

    public static void SaveUndoStep(IUndoable action)
    {
        Debug.Log("Saving action");
        undoHistory.Push(action);
        redoHistory.Clear();
    }

    /// <summary> Revert changes done by latest action, move it to redo stack. </summary>
    public static void UndoAction()
    {
        Debug.Log("Undo attempt");
        if (undoHistory.Count == 0) { return; }
        Debug.Log("Undo success");

        Debug.Log(undoHistory.Count);

        IUndoable action = undoHistory.Pop();
        action.Undo(); redoHistory.Push(action);
    }

    /// <summary> Do first redoable action again, move it to undo stack. </summary>
    public static void RedoAction()
    {
        Debug.Log("Redo attempt");
        if (redoHistory.Count == 0) { return; }
        Debug.Log("Redo success");

        Debug.Log(redoHistory.Count);

        IUndoable action = redoHistory.Pop();
        action.Redo(); undoHistory.Push(action);
    }


    /// <summary> Returns the estimated total bytes taken up by saved actions. </summary>
    public static int GetTotalSize()
    {
        int totalSize = 0;

        foreach (IUndoable action in undoHistory) { totalSize += action.GetSize(); }
        foreach (IUndoable action in redoHistory) { totalSize += action.GetSize(); }

        return totalSize;
    }

    #endregion Public Functions
}
