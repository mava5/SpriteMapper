
// Code help from: https://discussions.unity.com/t/custom-inspector-to-capture-a-single-key-press/713848/6

using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using SpriteMapper;


public static class EditorInputControls
{
    private static Shortcut currentlyReadShortcut;
    private static bool reading = false;
    private static int focusedControl = 0;

    private static Dictionary<KeyCode, bool> heldModifierKeys = new()
    {
        { KeyCode.LeftShift,    false }, { KeyCode.RightShift,   false },
        { KeyCode.LeftControl,  false }, { KeyCode.RightControl, false },
        { KeyCode.LeftAlt,      false }, { KeyCode.RightAlt,     false },
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
    /// <br/>   Only returns modified shortcut once reading has finished.
    /// </summary>
    public static Shortcut ShortcutField(Rect controlRect, Shortcut startShortcut)
    {
        int control = GUIUtility.GetControlID(FocusType.Keyboard);

        Event evt = Event.current;
        EventType evtType = evt.GetTypeForControl(control);

        // Repaint control
        if (evtType == EventType.Repaint)
        {
            GUIStyle style = GUI.skin.GetStyle("TextField");

            if (control == focusedControl)
            {
                string view = controlRect.width < 100 ? currentlyReadShortcut.ShortView : currentlyReadShortcut.View;
                style.Draw(controlRect, new GUIContent(view), control);

                // Draw red line in front of shortcut field when it's focused
                controlRect.width = 1;
                EditorGUI.DrawRect(controlRect, Color.HSVToRGB(0, 0.8f, 1));
                controlRect.x += 1;
                EditorGUI.DrawRect(controlRect, Color.HSVToRGB(0, 0.8f, 0.75f));
            }
            else
            {
                string view = controlRect.width < 100 ? startShortcut.ShortView : startShortcut.View;
                style.Draw(controlRect, new GUIContent(view), control);
            }

            return startShortcut;
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

                    currentlyReadShortcut = new();

                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
            }

            return startShortcut;
        }

        // Only read key presses for the focused shortcut field
        if (control != focusedControl) { return startShortcut; }


        bool finished = false;
        if (evt.isKey)
        {
            finished = HandleKeyboardInput(ref currentlyReadShortcut, ref evt, evtType);
        }
        else if (evt.isMouse)
        {
            finished = HandleMouseInput(ref currentlyReadShortcut, ref evt, evtType);
        }

        if (finished)
        {
            StopReadingShortcut();
            return currentlyReadShortcut;
        }
        else
        {
            return startShortcut;
        }
    }


    #region Private Methods ======================================================================= Private Methods

    /// <summary>
    /// <br/>   Reads keyboard key and updates referenced shortcut accordingly.
    /// <br/>   Uses referenced event to prevent Unity from picking it up.
    /// <br/>   Returns a boolean, which tells if reading finished.
    /// </summary>
    private static bool HandleKeyboardInput(ref Shortcut shortcut, ref Event evt, EventType evtType)
    {
        bool finished = false;

        if (evtType == EventType.KeyDown)
        {
            if (heldModifierKeys.ContainsKey(evt.keyCode))
            {
                heldModifierKeys[evt.keyCode] = true;
                UpdateShortcutModifierKeys(ref shortcut);
            }
            else if (evt.keyCode == KeyCode.Escape)
            {
                shortcut.Binding = "";
                finished = true;
            }
            else
            {
                shortcut.Binding = "<Keyboard>/" + evt.keyCode;
                finished = true;
            }
        }
        else if (evtType == EventType.KeyUp)
        {
            if (heldModifierKeys.ContainsKey(evt.keyCode))
            {
                heldModifierKeys[evt.keyCode] = false;
                UpdateShortcutModifierKeys(ref shortcut);
            }
        }

        evt.Use();
        return finished;
    }

    /// <summary>
    /// <br/>   Reads mouse button and updates referenced shortcut accordingly.
    /// <br/>   Uses referenced event to prevent Unity from picking it up.
    /// <br/>   Returns a boolean, which tells if reading finished.
    /// </summary>
    private static bool HandleMouseInput(ref Shortcut shortcut, ref Event evt, EventType evtType)
    {
        bool finished = false;

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
            finished = true;
        }

        evt.Use();
        return finished;
    }


    /// <summary> Updates referenced shortcut's modifier key booleans based on held modifier keys. </summary>
    private static void UpdateShortcutModifierKeys(ref Shortcut shortcut)
    {
        shortcut.Shift =    heldModifierKeys[KeyCode.LeftShift]     || heldModifierKeys[KeyCode.RightShift];
        shortcut.Ctrl =     heldModifierKeys[KeyCode.LeftControl]   || heldModifierKeys[KeyCode.RightControl]   || heldModifierKeys[KeyCode.AltGr];
        shortcut.Alt =      heldModifierKeys[KeyCode.LeftAlt]       || heldModifierKeys[KeyCode.RightAlt]       || heldModifierKeys[KeyCode.AltGr];
    }

    #endregion Private Methods
}
