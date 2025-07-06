
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo
    {
        /// <summary> The <see cref="Context"/>, in which action can be used. </summary>
        public readonly string ActionContext;

        /// <summary> A high priority action succeeding disregards the lower priority ones. </summary>
        public readonly PriorityLevel Priority;

        /// <summary> The type of the action the info points to. </summary>
        public readonly Type ActionType;

        /// <summary> Explanation for how the action works, used for action's tooltip. </summary>
        public readonly string Description;

        public readonly bool IsLong;
        public readonly bool IsShort;
        public readonly bool IsUndoable;
        public readonly bool IsUserExecutable;

        /// <summary> Shortcut for executing an user executable action. </summary>
        public Shortcut Shortcut { get; private set; } = null;


        public ActionInfo(SerializedActionInfo serializedInfo)
        {
            ActionContext = serializedInfo.ActionContext;
            ActionType = Type.GetType(serializedInfo.ActionFullName);
            Description = serializedInfo.Description;

            IsLong = serializedInfo.IsLong;
            IsUndoable = serializedInfo.IsUndoable;
            IsUserExecutable = serializedInfo.IsUserExecutable;

            Shortcut = serializedInfo.Shortcut;
        }


        /// <summary> Rebinds the action's shortcut. </summary>
        public void Rebind(Shortcut newShortcut)
        { if (IsUserExecutable) { Shortcut = newShortcut; } }
    }
}
