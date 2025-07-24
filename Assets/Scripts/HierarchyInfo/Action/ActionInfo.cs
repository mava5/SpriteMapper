
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo
    {
        /// <summary> The type of the action the info points to. </summary>
        public readonly Type ActionType = null;

        /// <summary> The context in which action can be used. Determined by the action's namespace. </summary>
        public readonly string Context = "";

        public readonly string Description = "";
        
        public readonly ActionSettings Settings = null;
        public readonly bool IsUndoable = false;

        public readonly Shortcut DefaultShortcut = null;
        public Shortcut Shortcut { get; private set; } = null;

        /// <summary> Determines if action can be used. </summary>
        public bool Active { get; private set; } = true;


        public ActionInfo(SerializedActionInfo serializedInfo)
        {
            ActionType = Type.GetType(serializedInfo.FullName);
            Context = serializedInfo.Context;
            Description = serializedInfo.Description;

            Settings = serializedInfo.Settings;
            IsUndoable = serializedInfo.IsUndoable;

            // TODO: Read saved shortcut from a text file
            Shortcut = serializedInfo.DefaultShortcut;
            DefaultShortcut = serializedInfo.DefaultShortcut;
        }


        public void RebindAction(Shortcut shortcut)
        {
            if (Settings.ShortcutState == ActionShortcutState.Exists && Active)
            {
                Shortcut = shortcut;
            }
        }

        public bool IsExecutableInContext(string contextToExecuteIn, bool isContextOverwrittenByLongAction)
        {
            bool isGlobal = Context == "Global" || Context.StartsWith("Global.");
            bool canExecute = false;

            switch (Settings.DescendantUsability)
            {
                case ActionDescendantUsability.None:
                    canExecute = isGlobal || Context == contextToExecuteIn; break;

                case ActionDescendantUsability.Limited:
                    canExecute = isContextOverwrittenByLongAction ?
                        Context == contextToExecuteIn :
                        isGlobal || Context.StartsWith(contextToExecuteIn); break;

                case ActionDescendantUsability.Full:
                    canExecute = isGlobal || Context.StartsWith(contextToExecuteIn); break;
            }

            return canExecute;
        }
    }
}
