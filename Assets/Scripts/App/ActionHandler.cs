
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles controls and <see cref="Action"/> related parts of app.
    /// <br/>   
    /// <br/>   Takes care of:
    /// <br/>   • controls initialization based on each action's <see cref="Shortcut"/>
    /// <br/>   • action creation, updating and disposal
    /// <br/>   • shortcut rebinding
    /// </summary>
    public class ActionHandler
    {
        /// <summary> Mouse position captured before starting a long action. </summary>
        public Vector2 StartMousePosition { get; private set; } = Vector2.zero;

        // Contains the currently active ILong action
        // Only one long action can be active at once
        // For example the user cannot quick use a tool while panning a camera
        private ILong currentLongAction = null;

        // Keep track of actions to execute based on their shortcut
        // Lower priority actions will be disgarded if higher priority ones succeeded
        private Dictionary<Shortcut, Queue<(ActionInfo info, int priority)>> actionQueues = new();

        private bool shiftHeld = false;
        private bool ctrlHeld = false;
        private bool altHeld = false;


        #region Initialization ============================================================================== Initialization

        /// <summary> Creates an <see cref="InputAction"/> for each stored <see cref="ActionInfo"/>. </summary>
        public void Initialize()
        {
            InputActionMap actionMap = new("Shortcuts");

            foreach (ActionInfo info in HierarchyInfoDictionary.ActionInfos.Values)
            {
                if (!info.IsShortcutExecutable) { continue; }

                InputAction inputAction = actionMap.AddAction(info.ActionType.FullName, InputActionType.Button, info.Shortcut.Binding);
                inputAction.performed += callbackContext => { OnActionShortcutDown(callbackContext, info); };

                if (info.IsLong)
                {
                    inputAction.canceled += callbackContext => { OnLongActionShortcutUp(callbackContext, info); };
                }
            }

            foreach (InputAction action in actionMap.actions) { action.Enable(); }
        }

        #endregion Initialization


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Handles action and controls variables updating. </summary>
        public void Update()
        {
            // Execute highest priority actions from each queue
            foreach ((Shortcut shortcut, Queue<(ActionInfo info, int priority)> queue) in actionQueues)
            {
                queue.OrderByDescending(queuedAction => queuedAction.priority);

                // Go through each action in queue until one succeeds or begins successfully
                while (queue.Count > 0)
                {
                    ActionInfo info = queue.Dequeue().info;

                    if (info.Context != App.CurrentContext) { continue; }

                    Action action = (Action)Activator.CreateInstance(info.ActionType);

                    if (info.IsShort && ((IShort)action).Do()) { break; }
                    if (info.IsLong && ((ILong)action).Begin())
                    {
                        if (currentLongAction != null) { currentLongAction.Cancel(); }
                        StartMousePosition = Input.mousePosition;
                        currentLongAction = (ILong)action;
                        break;
                    }
                }
            }
            actionQueues.Clear();

            // Evaluate currently active long action
            if (currentLongAction != null)
            {
                if (currentLongAction.CancelPredicate)
                {
                    currentLongAction.Cancel();
                    currentLongAction = null;
                }
                else if (currentLongAction.EndPredicate)
                {
                    currentLongAction.End();
                    currentLongAction = null;
                }
                else
                {
                    currentLongAction.Update();
                }
            }

            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }


        /// <summary> Puts <see cref="Action"/> based on given type at the front of the action queue. </summary>
        /// <param name="shortcut"> The actions that share the same shortcut get ignored </param>
        public void ForceExecute<T>(Shortcut shortcut) where T : Action
        {
            actionQueues[shortcut].Enqueue((HierarchyInfoDictionary.ActionInfos[typeof(T)], int.MaxValue));
        }

        /// <summary> Puts <see cref="Action"/> based on given type at the front of the action queue. </summary>
        /// <param name="shortcut"> The actions that share the same shortcut get ignored </param>
        public void ForceExecute(Shortcut shortcut, Type actionType)
        {
            actionQueues[shortcut].Enqueue((HierarchyInfoDictionary.ActionInfos[actionType], int.MaxValue));
        }

        /// <summary> Puts <see cref="Action"/> based on given info at the front of the action queue. </summary>
        /// <param name="shortcut"> The actions that share the same shortcut get ignored </param>
        public void ForceExecute(Shortcut shortcut, ActionInfo info)
        {
            actionQueues[shortcut].Enqueue((info, int.MaxValue));
        }

        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods

        private void OnActionShortcutDown(InputAction.CallbackContext context, ActionInfo info)
        {
            if ((info.Shortcut.Shift && !shiftHeld) ||
                (info.Shortcut.Ctrl && !ctrlHeld) ||
                (info.Shortcut.Alt && !altHeld)) { return; }

            actionQueues[info.Shortcut].Enqueue((info, (int)info.Priority));
        }

        private void OnLongActionShortcutUp(InputAction.CallbackContext context, ActionInfo info)
        {
            if (currentLongAction != null && currentLongAction.GetType() == info.ActionType)
            {
                currentLongAction.ShortcutReleased = true;
            }
        }

        #endregion Private Methods
    }
}
