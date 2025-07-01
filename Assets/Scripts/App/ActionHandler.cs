
using System;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Handles the creation, updating and disposal of each <see cref="Action"/>. </summary>
    public class ActionHandler
    {
        private List<ILong> longActions = new();


        #region Public Methods ========================================================== Public Methods

        /// <summary> Updates each active <see cref="ILong"/> <see cref="Action"/>. </summary>
        public void Update()
        {
            // Handle long Actions
            for (int i = 0; i < longActions.Count; i++)
            {
                ILong action = longActions[i];

                // Cancel or end long action based on its corresponding predicates
                if (action.CancelPredicate) { action.Cancel(); longActions.RemoveAt(i--); continue; }
                else if (action.EndPredicate) { action.End(); longActions.RemoveAt(i--); continue; }

                action.Update();
            }
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

            if (info.IsLong) { longActions.Add((ILong)action); }

            return action;
        }

        #endregion Public Methods
    }
}
