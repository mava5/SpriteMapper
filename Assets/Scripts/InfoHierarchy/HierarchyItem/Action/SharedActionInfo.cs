
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo : HierarchyItemInfo
    {
        public readonly ActionSettings Settings = null;

        /// <summary> The type of the action the info points to. </summary>
        public readonly Type ActionType;

        public readonly bool IsUndoable;

        public readonly Shortcut DefaultShortcut = null;
        public Shortcut Shortcut { get; private set; } = null;

        /// <summary> Determines if action can be used. </summary>
        public bool Active { get; private set; } = true;


        public ActionInfo(ActionSettings settings)
        {
            Settings = settings;

            ActionType = Type.GetType(serializedInfo.FullName);
            IsUndoable = serializedInfo.IsUndoable;

            // TODO: Read saved shortcut from a text file
            Shortcut = serializedInfo.DefaultShortcut;
            DefaultShortcut = serializedInfo.DefaultShortcut;
        }


        public void RebindAction(Shortcut shortcut)
        {
            if (Settings.ShortcutState == ActionShortcutState.Rebindable && Active)
            {
                Shortcut = shortcut;
            }
        }

        public bool ExecutableIn(Context executionContext)
        {
            bool isGlobal = Context.IsWithin(App.Hierarchy.GlobalContext);
            bool canExecute = false;

            switch (Settings.DescendantUsability)
            {
                case ActionDescendantUsability.None:
                    canExecute = isGlobal || Context == executionContext; break;

                case ActionDescendantUsability.Limited:
                    canExecute = Context.CanAccess(executionContext); break;

                case ActionDescendantUsability.Full:
                    canExecute = isGlobal || Context.IsWithin(executionContext); break;
            }

            return canExecute;
        }
    }
}
