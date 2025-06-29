
// Code help from: https://discussions.unity.com/t/custom-inspector-to-capture-a-single-key-press/713848/6

using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using SpriteMapper;


public static class EditorInputControls
{
    private static bool reading = false;
    private static int focusedControl = 0;

    private static Dictionary<KeyCode, bool> heldModifierKeys = new()
    {
        { KeyCode.LeftShift,    false }, { KeyCode.RightShift,   false },
        { KeyCode.LeftControl,  false }, { KeyCode.RightControl, false },
        { KeyCode.LeftAlt,      false }, { KeyCode.RightAlt,     false },
        { KeyCode.LeftCommand,  false }, { KeyCode.RightCommand, false },
        { KeyCode.AltGr,        false },
    };


    /// <summary> Force ends the current shortcut reading </summary>
    public static void StopReadingShortcut()
    {
        if (reading) { GUIUtility.keyboardControl = 0; }
        if (GUIUtility.hotControl == focusedControl) { GUIUtility.hotControl = 0; }

        heldModifierKeys.Keys.ToList().ForEach(key => heldModifierKeys[key] = false);

        reading = false; focusedControl = 0;
    }

    /// <summary>
    /// <br/>   Draws a text field with given rect for inputting a <see cref="Shortcut"/>.
    /// <br/>   Will start listening for a keyboard or mouse inputs when selected.
    /// <br/>   Returns boolean, which tells if the shortcut field is focused.
    /// <br/>   Also returns the modified shortcut.
    /// </summary>
    public static (bool, Shortcut) ShortcutField(Rect controlRect, Shortcut shortcut)
    {
        int control = GUIUtility.GetControlID(FocusType.Keyboard);

        Event evt = Event.current;
        EventType evtType = evt.GetTypeForControl(control);


        // Repaint control
        if (evtType == EventType.Repaint)
        {
            GUIStyle style = GUI.skin.GetStyle("TextField");
            style.Draw(controlRect, new GUIContent(shortcut.View), control);
            
            return (control == focusedControl, shortcut);
        }


        // Focus shortcut field
        if (!reading)
        {
            if (evtType == EventType.MouseDown)
            {
                if (controlRect.Contains(evt.mousePosition) &&
                    evt.button == 0 && GUIUtility.hotControl == 0)
                {
                    GUIUtility.hotControl = control;
                    GUIUtility.keyboardControl = control;
                    evt.Use();
                }
            }
            if (evtType == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == control)
                {
                    reading = true;
                    focusedControl = control;

                    // Reset shortcut
                    shortcut = new();

                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
            }

            return (control == focusedControl, shortcut);
        }


        // Stop reading if escape key is pressed
        if (evt.isKey && evtType == EventType.KeyUp && evt.keyCode == KeyCode.Escape)
        {
            StopReadingShortcut();
            return (false, shortcut);
        }

        // Skip controls that aren't focused
        if (control != focusedControl) { return (false, shortcut); }


        if (evt.isKey) { HandleKeyboardInput(ref shortcut, ref evt, evtType); }
        else if (evt.isMouse) { HandleMouseInput(ref shortcut, ref evt, evtType); }

        return (control == focusedControl, shortcut);
    }


    #region Private Methods ======================================================================= Private Methods

    /// <summary>
    /// <br/>   Reads keyboard key and updates referenced shortcut accordingly.
    /// <br/>   Uses referenced event to prevent Unity from picking it up.
    /// </summary>
    private static void HandleKeyboardInput(ref Shortcut shortcut, ref Event evt, EventType evtType)
    {
        if (evtType == EventType.KeyDown)
        {
            if (heldModifierKeys.ContainsKey(evt.keyCode))
            {
                heldModifierKeys[evt.keyCode] = true;
                UpdateShortcutModifierKeys(ref shortcut);
            }
        }
        else if (evtType == EventType.KeyUp)
        {
            if (heldModifierKeys.ContainsKey(evt.keyCode))
            {
                heldModifierKeys[evt.keyCode] = false;
                UpdateShortcutModifierKeys(ref shortcut);
            }
            else
            {
                shortcut.Binding = "<Keyboard>/" + evt.keyCode;
                StopReadingShortcut();
            }
        }

        evt.Use();
    }

    /// <summary>
    /// <br/>   Reads mouse button and updates referenced shortcut accordingly.
    /// <br/>   Uses referenced event to prevent Unity from picking it up.
    /// </summary>
    private static void HandleMouseInput(ref Shortcut shortcut, ref Event evt, EventType evtType)
    {
        if (evtType == EventType.MouseUp)
        {
            switch (evt.button)
            {
                case 0: StopReadingShortcut(); shortcut.Binding = "<Mouse>/LeftButton"; break;
                case 1: StopReadingShortcut(); shortcut.Binding = "<Mouse>/RightButton"; break;
                case 2: StopReadingShortcut(); shortcut.Binding = "<Mouse>/MiddleButton"; break;
                case 3: StopReadingShortcut(); shortcut.Binding = "<Mouse>/Back"; break;
                case 4: StopReadingShortcut(); shortcut.Binding = "<Mouse>/Forward"; break;
            }
        }

        evt.Use();
    }


    /// <summary> Updates referenced shortcut's modifier key booleans based on held modifier keys. </summary>
    private static void UpdateShortcutModifierKeys(ref Shortcut shortcut)
    {
        shortcut.Shift =    heldModifierKeys[KeyCode.LeftShift]     || heldModifierKeys[KeyCode.RightShift];
        shortcut.Ctrl =     heldModifierKeys[KeyCode.LeftControl]   || heldModifierKeys[KeyCode.RightControl]   || heldModifierKeys[KeyCode.AltGr];
        shortcut.Alt =      heldModifierKeys[KeyCode.LeftAlt]       || heldModifierKeys[KeyCode.RightAlt]       || heldModifierKeys[KeyCode.AltGr];
        shortcut.Cmd =      heldModifierKeys[KeyCode.LeftCommand]   || heldModifierKeys[KeyCode.RightCommand];
    }

    #endregion Private Methods
}
