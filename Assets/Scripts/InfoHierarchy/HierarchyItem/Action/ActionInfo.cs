
using System;
using System.Reflection;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo<T> : HierarchyItemInfo<T> where T : Action
    {
        public ActionSettings Settings => BaseSettings as ActionSettings;

        /// <summary> Determines if action can be used. </summary>
        public bool Active { get; private set; } = true;

        public readonly bool IsUndoable;

        public readonly Shortcut DefaultShortcut = null;
        public Shortcut Shortcut { get; private set; } = null;


        public ActionInfo(Shortcut defaultShortcut, string nameOverwrite = "", string descriptionOverwrite = "")
            : base(nameOverwrite, descriptionOverwrite)
        {
            IsUndoable = typeof(IUndoable).IsAssignableFrom(typeof(T));
            
            // TODO: Read saved shortcut from a text file
            Shortcut = defaultShortcut;
            DefaultShortcut = defaultShortcut;
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
