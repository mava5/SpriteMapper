
using System.Collections.Generic;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A static class, which handles the <see cref="Action"/> stack.
    /// <br/>   Used to keep track of <see cref="IUndoable"/> <see cref="Action"/> history.
    /// </summary>
    public static class ActionHistory
    {
        /// <summary>
        /// <br/>   Keeps track of actions that can be redone.
        /// <br/>   Top element of stack is the first redoable action.
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
            undoHistory.Push(action);
            redoHistory.Clear();
        }

        /// <summary> Revert changes done by latest action, move it to redo stack. </summary>
        public static void UndoAction()
        {
            if (undoHistory.Count == 0) { return; }

            IUndoable action = undoHistory.Pop();
            action.Undo(); redoHistory.Push(action);
        }

        /// <summary> Do first redoable action again, move it to undo stack. </summary>
        public static void RedoAction()
        {
            if (redoHistory.Count == 0) { return; }

            IUndoable action = redoHistory.Pop();
            action.Redo(); undoHistory.Push(action);
        }


        /// <summary> Returns the estimated total bytes taken up by saved actions. </summary>
        public static int GetTotalSize()
        {
            int totalSize = 0;

            foreach (IUndoable action in undoHistory) { totalSize += action.GetMemorySize(); }
            foreach (IUndoable action in redoHistory) { totalSize += action.GetMemorySize(); }

            return totalSize;
        }

        #endregion Public Functions
    }
}
