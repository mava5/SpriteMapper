
using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles <see cref="IUserExecutable"/> <see cref="Action"/> creation based on its <see cref="Shortcut"/>.
    /// <br/>   Also handles shortcut rebinding.
    /// </summary>
    public class ControlsHandler
    {
        private bool shiftHeld = false;
        private bool ctrlHeld = false;
        private bool altHeld = false;
        private bool cmdHeld = false;


        #region Initialization ============================================================================== Initialization

        /// <summary> Creates an <see cref="InputAction"/> for each stored <see cref="ActionInfo"/>. </summary>
        public ControlsHandler()
        {
            InputActionMap actionMap = new("Shortcuts");

            foreach (ActionInfo info in ActionInfoDictionary.Data.Values)
            {
                InputAction inputAction =
                    actionMap.AddAction(info.ActionType.FullName, InputActionType.Button, info.Shortcut.Binding);

                inputAction.performed += callbackContext => { OnActionPerformed(callbackContext, info); };
            }

            foreach (InputAction action in actionMap.actions) { action.Enable(); }
        }

        #endregion Initialization


        #region Public Methods ============================================================================== Public Methods
        
        /// <summary> Updates control variables. </summary>
        public void Update()
        {
            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            cmdHeld = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
        }

        #endregion Public Methods


        #region Private Methods ============================================================================= Private Methods

        private void OnActionPerformed(InputAction.CallbackContext callbackContext, ActionInfo info)
        {
            if (!callbackContext.performed) { return; }

            if ((info.Shortcut.Shift && !shiftHeld) ||
                (info.Shortcut.Ctrl && !ctrlHeld) ||
                (info.Shortcut.Alt && !altHeld) ||
                (info.Shortcut.Cmd && !cmdHeld)) { return; }

            App.Action.Create(info);
        }

        #endregion Private Methods
    }
}
