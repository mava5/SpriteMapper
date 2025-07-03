
using System;
using System.Collections.Generic;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles the creation, updating and disposal of each <see cref="Action"/>.
    /// <br/>   Also keeps track of the undo / redo history with an <see cref="ActionHistory"/>.
    /// </summary>
    public class ActionHandler
    {
        public ActionHistory History { get; private set; } = new();


        // Contains currently active ILong Actions
        // Only one long action of a specific type can run at once
        private Dictionary<Type, ILong> activeLongActions = new();


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Updates each active <see cref="ILong"/> <see cref="Action"/>. </summary>
        public void Update()
        {
            List<Type> longActionsToRemove = new();
            foreach ((Type type, ILong action) in activeLongActions)
            {
                // Cancel or end long action based on its corresponding predicates
                if (action.CancelPredicate) { action.Cancel(); longActionsToRemove.Add(type); continue; }
                else if (action.EndPredicate) { action.End(); longActionsToRemove.Add(type); continue; }

                action.Update();
            }

            // Remove long actions that ended or got cancelled
            foreach (Type type in longActionsToRemove) { activeLongActions.Remove(type); }
        }


        /// <summary> Creates and runs action of given type with provided arguments. </summary>
        public Action Create<T>(object[] args = null) where T : Action
        {
            return Create(ActionInfoDictionary.Instance[typeof(T)], args);
        }

        /// <summary> Creates and runs action based on given info and arguments. </summary>
        public Action Create(ActionInfo info, object[] args = null)
        {
            Action action = (Action)Activator.CreateInstance(info.ActionType, args);

            if (info.IsLong) { activeLongActions.Add(info.ActionType, (ILong)action); }

            return action;
        }


        /// <summary>  </summary>
        public void OnShortcutPerformed(ActionInfo info)
        {

        }

        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods



        #endregion Private Methods
    }
}
