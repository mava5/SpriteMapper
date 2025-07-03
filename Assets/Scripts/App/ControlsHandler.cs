
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
        
        /// <summary> Updates control variables. </summary>
        public void Update()
        {
            shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
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
