
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo
    {
        /// <summary> The type of the action the info points to. </summary>
        public readonly Type ActionType;

        /// <summary> The context in which action can be used. Determined by the action's namespace. </summary>
        public readonly string Context;

        /// <summary> Explanation for how the action works, used for action's tooltip. </summary>
        public readonly string Description;

        /// <summary> A high priority action succeeding causes the lower priority ones to get ignored. </summary>
        public readonly PriorityLevel Priority;

        public readonly bool IsLong;
        public readonly bool IsShort;
        public readonly bool IsUndoable;
        public readonly bool IsShortcutExecutable;

        /// <summary> Shortcut for executing the action. </summary>
        public Shortcut Shortcut { get; private set; } = null;


        public ActionInfo(SerializedActionInfo serializedInfo)
        {
            ActionType = Type.GetType(serializedInfo.FullName);
            Context = serializedInfo.Context;
            Description = serializedInfo.Description;

            IsLong = serializedInfo.IsLong;
            IsUndoable = serializedInfo.IsUndoable;
            IsShortcutExecutable = serializedInfo.IsShortcutExecutable;

            Shortcut = serializedInfo.Shortcut;
        }


        /// <summary> Rebinds the action's shortcut. </summary>
        public void Rebind(Shortcut newShortcut)
        { if (IsShortcutExecutable) { Shortcut = newShortcut; } }
    }
}
