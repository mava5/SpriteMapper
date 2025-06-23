
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


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
    }


    public static void AddToUpdateList(ILong action) { actions.Add(action); }


    public void Begin(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        new Actions.DrawImage.Draw().Begin();
    }
}
