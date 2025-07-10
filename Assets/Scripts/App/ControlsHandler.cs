
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles app controls and <see cref="Shortcut"/> activation.
    /// <br/>   Based on activated shortcuts, adds each corresponding <see cref="Action"/> to <see cref="ActionHandler"/> queue.
    /// </summary>
    public class ControlsHandler
    {
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

                InputAction inputAction = actionMap.AddAction(info.ActionType.FullName, InputActionType.Button, info.DefaultShortcut1.Binding);
                inputAction.performed += callbackContext => { OnActionShortcutDown(callbackContext, info.DefaultShortcut1, info); };

                if (info.IsLong)
                {
                    inputAction.canceled += callbackContext => { OnLongActionShortcutUp(callbackContext, info); };
                }
            }

            foreach (InputAction action in actionMap.actions) { action.Enable(); }
        }

        #endregion Initialization


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Updates modifier key variables. </summary>
        public void UpdateModifierKeys()
        {
            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods

        private void OnActionShortcutDown(InputAction.CallbackContext context, Shortcut shortcut, ActionInfo info)
        {
            Debug.Log(info.ActionType.FullName);

            if ((shortcut.Shift && !shiftHeld) ||
                (shortcut.Ctrl && !ctrlHeld) ||
                (shortcut.Alt && !altHeld)) { return; }

            App.Actions.AddToQueue(info);
        }

        private void OnLongActionShortcutUp(InputAction.CallbackContext context, ActionInfo info)
        {
            if (App.Actions.ActiveLongActions.ContainsKey(info.ActionType))
            {
                App.Actions.ActiveLongActions[info.ActionType].longAction.ShortcutReleased = true;
            }
        }

        #endregion Private Methods
    }
}
