
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    public class TestInput : MonoBehaviour
    {
        private static List<ILong> actions = new();


        private void Update()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                ILong action = actions[i];

                // Cancel or end action based on its corresponding predicates
                if (action.CancelPredicate) { action.Cancel(); actions.RemoveAt(i--); continue; }
                else if (action.EndPredicate) { action.End(); actions.RemoveAt(i--); continue; }

                action.Update();
            }



            // TODO: DEBUG - Testing out drawing
            //Image.Update();

            if (Input.GetKeyUp(KeyCode.E)) { Debug.Log(ActionHistory.GetTotalSize()); }
        }


        public static void AddToUpdateList(ILong action) { actions.Add(action); }


        public void Draw(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            Actions.Draw action = new();
            Debug.Log(action.IsLong + ", " + action.IsUndoable + ", " + action.Context);
        }

        public void Flip(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            Actions.Flip action = new();
            Debug.Log(action.IsLong + ", " + action.IsUndoable + ", " + action.Context);
        }

        public void Undo(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            ActionHistory.UndoAction();
        }

        public void Redo(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            ActionHistory.RedoAction();
        }
    }
}
