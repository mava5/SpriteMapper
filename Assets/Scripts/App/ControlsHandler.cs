
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles <see cref="Action"/> and controls related parts of app.
    /// <br/>    
    /// <br/>   Takes care of:
    /// <br/>   • action creation, updating and disposal
    /// <br/>   • controls initialization based on each action's <see cref="Shortcut"/>
    /// <br/>   • shortcut rebinding
    /// </summary>
    public class ActionHandler
    {
        // Contains currently active ILong actions
        // Only one long action of a specific type can run at once
        private Dictionary<Type, ILong> activeLongActions = new();

        // Keep track of actions to execute based on their shortcut
        // Lower priority actions will be disgarded if higher priority ones succeeded
        private Dictionary<Shortcut, Queue<ActionInfo>> actionQueues = new();

        private bool shiftHeld = false;
        private bool ctrlHeld = false;
        private bool altHeld = false;


        #region Initialization ============================================================================== Initialization

        /// <summary> Creates an <see cref="InputAction"/> for each stored <see cref="ActionInfo"/>. </summary>
        public void Initialize()
        {
            InputActionMap actionMap = new("Shortcuts");

            //AddModifierKeyInputActions(ref actionMap);

            foreach (ActionInfo info in ActionInfoDictionary.Data.Values)
            {
                Debug.Log(info.ActionType + ", " + info.Shortcut.Binding);
                InputAction inputAction =
                    actionMap.AddAction(info.ActionType.FullName, InputActionType.Button, info.Shortcut.Binding);

                //inputAction.performed += callbackContext => { OnActionPerformed(callbackContext, info); };
                //inputAction.canceled += callbackContext => { OnActionPerformed(callbackContext, info); };
            }

            foreach (InputAction action in actionMap.actions) { action.Enable(); }
        }

        #endregion Initialization


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Updates controls variables and each active <see cref="ILong"/> <see cref="Action"/>. </summary>
        public void Update()
        {
            // Execute highest priority actions from each queue
            foreach ((Shortcut shortcut, Queue<ActionInfo> queue) in actionQueues)
            {
                queue.OrderBy(info => info.Priority).ThenBy(info => info.ActionType.FullName);

                // Go through each action in queue until one succeeds or begins successfully
                foreach (ActionInfo info in queue)
                {
                    if (info.IsShort && ((IShort)Create(queue.Dequeue())).Do()) { break; }
                    if (info.IsLong && ((ILong)Create(queue.Dequeue())).Begin()) { break; }
                }
            }
            actionQueues.Clear();


            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);


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


        public void CancelLongActions()
        {
            foreach (ILong action in activeLongActions.Values) { action.Cancel(); }
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

        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods

        private void OnActionShortcutDown(InputAction.CallbackContext context, ActionInfo info)
        {
            //if (!callbackContext.performed) { return; }

            if ((info.Shortcut.Shift && !shiftHeld) ||
                (info.Shortcut.Ctrl && !ctrlHeld) ||
                (info.Shortcut.Alt && !altHeld)) { return; }

            //App.Action.Create(info);
        }

        //private void OnModifierKeyDown(InputAction.CallbackContext context)

        
        private void AddModifierKeyInputActions(ref InputActionMap actionMap, string modifierKey)
        {

        }

        #endregion Private Methods
    }
}
