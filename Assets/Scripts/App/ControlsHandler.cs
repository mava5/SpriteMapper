
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

            foreach (ActionInfo actionInfo in HierarchyInfo.ActionInfos.Values)
            {
                if (actionInfo.Settings.ShortcutState == ActionShortcutState.None) { continue; }


                InputAction inputAction = actionMap.AddAction(actionInfo.Type.FullName,
                    InputActionType.Button, actionInfo.Shortcut.Binding);
                
                inputAction.performed += callbackContext => { OnActionShortcutDown(actionInfo); };
                inputAction.canceled += callbackContext => { OnActionShortcutUp(actionInfo); };
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

        private void OnActionShortcutDown(ActionInfo actionInfo)
        {
            Debug.Log("Pressed: " + actionInfo.Type.FullName);

            if ((actionInfo.Shortcut.Shift && !shiftHeld) ||
                (actionInfo.Shortcut.Ctrl && !ctrlHeld) ||
                (actionInfo.Shortcut.Alt && !altHeld)) { return; }

            App.Actions.TryAddToQueue(actionInfo);
        }

        private void OnActionShortcutUp(ActionInfo actionInfo)
        {
            if (shortcutsReleasedThisFrame.Contains(actionInfo.Shortcut)) { return; }

            Debug.Log("Released: " + actionInfo.Type.FullName);

            if ((actionInfo.Shortcut.Shift && !shiftHeld) ||
                (actionInfo.Shortcut.Ctrl && !ctrlHeld) ||
                (actionInfo.Shortcut.Alt && !altHeld)) { return; }

            shortcutsReleasedThisFrame.Add(actionInfo.Shortcut);
            App.Actions.ReleaseInput(actionInfo.Shortcut);
        }

        #endregion Private Methods
    }
}
