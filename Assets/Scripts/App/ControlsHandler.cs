
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles app controls and <see cref="Shortcut"/> activation.
    /// <br/>   Based on pressed shortcuts, adds each corresponding <see cref="Action"/> to <see cref="ActionHandler"/> queue.
    /// </summary>
    public class ControlsHandler
    {
        private bool shiftHeld = false;
        private bool ctrlHeld = false;
        private bool altHeld = false;

        private HashSet<Shortcut> shortcutsReleasedThisFrame = new();


        #region Initialization ============================================================================== Initialization

        /// <summary> Creates an <see cref="InputAction"/> for each stored <see cref="ActionInfo"/>. </summary>
        public void Initialize()
        {
            InputActionMap actionMap = new("Shortcuts");

            foreach (ActionInfo info in HierarchyInfo.ActionInfos.Values)
            {
                if (info.Settings.ShortcutState == ActionShortcutState.None) { continue; }


                InputAction inputAction = actionMap.AddAction(info.ActionType.FullName, InputActionType.Button, info.Shortcut.Binding);
                
                inputAction.performed += callbackContext => { OnActionShortcutDown(info); };
                inputAction.canceled += callbackContext => { OnActionShortcutUp(info); };
            }

            actionMap.actionTriggered += ActionMap_actionTriggered;

            foreach (InputAction action in actionMap.actions) { action.Enable(); }
        }

        private void ActionMap_actionTriggered(InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
        }

        #endregion Initialization


        #region Public Methods ============================================================================== Public Methods

        public void UpdateVariables()
        {
            shortcutsReleasedThisFrame.Clear();

            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }
        
        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods

        private void OnActionShortcutDown(ActionInfo info)
        {
            Debug.Log("Pressed: " + info.ActionType.FullName);

            if ((info.Shortcut.Shift && !shiftHeld) ||
                (info.Shortcut.Ctrl && !ctrlHeld) ||
                (info.Shortcut.Alt && !altHeld)) { return; }

            App.Actions.AddToQueue(info);
        }

        private void OnActionShortcutUp(ActionInfo info)
        {
            if (shortcutsReleasedThisFrame.Contains(info.Shortcut)) { return; }

            Debug.Log("Released: " + info.ActionType.FullName);

            if ((info.Shortcut.Shift && !shiftHeld) ||
                (info.Shortcut.Ctrl && !ctrlHeld) ||
                (info.Shortcut.Alt && !altHeld)) { return; }

            shortcutsReleasedThisFrame.Add(info.Shortcut);
            App.Actions.ReleaseInput(info.Shortcut);
        }

        #endregion Private Methods
    }
}
