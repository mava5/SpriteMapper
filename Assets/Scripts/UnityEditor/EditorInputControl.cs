
// Part of code from: https://discussions.unity.com/t/custom-inspector-to-capture-a-single-key-press/713848/6

using UnityEngine;
using UnityEditor;


public static class EditorInputControls
{
    public static string Identifier { get; private set; } = "";
    public static bool Reading { get; private set; } = false;


    /// <summary>
    /// <br/>   Draws a text field with given rect.
    /// <br/>   Will start listening for a keyboard or mouse action when selected.
    /// <br/>   Returns pressed keyboard key or mouse button as a binding string.
    /// <br/>   For example: "&lt;Keyboard&gt;/F", "&lt;Mouse&gt;/LeftButton"
    /// </summary>
    public static string ActionBindingField(Rect controlRect, string startBinding, string identifier)
    {
        string newBinding = startBinding;

        int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

        Event evt = Event.current;
        EventType evtType = evt.GetTypeForControl(controlID);


        // Repaint
        if (evtType == EventType.Repaint)
        {
            GUIStyle style = GUI.skin.GetStyle("TextField");
            style.Draw(controlRect, new GUIContent(newBinding), controlID);
        }

        // Keyboard key
        else if (evt.isKey)
        {
            if (GUIUtility.keyboardControl == controlID)
            {
                Reading = false;
                newBinding = "<Keyboard>/" + evt.keyCode.ToString();

                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                evt.Use();
            }
        }
        
        // Mouse button
        else if (evt.isMouse)
        {
            // Focus action binding field
            if (!Reading)
            {
                if (evtType == EventType.MouseDown)
                {
                    if (controlRect.Contains(evt.mousePosition) &&
                        evt.button == 0 && GUIUtility.hotControl == 0)
                    {
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = controlID;
                        evt.Use();
                    }
                }
                else if (evtType == EventType.MouseUp)
                {
                    if (GUIUtility.hotControl == controlID)
                    {
                        Reading = true;
                        Identifier = identifier;

                        GUIUtility.hotControl = 0;
                        evt.Use();
                    }
                }
            }

            // Get new binding based on pressed mouse button
            else
            {
                bool bindingRead = false;

                switch (evt.button)
                {
                    // Left mouse button
                    case 0: bindingRead = true; newBinding = "<Mouse>/LeftButton"; break;

                    // Right mouse button
                    case 1: bindingRead = true; newBinding = "<Mouse>/RightButton"; break;

                    // Middle mouse button
                    case 2: bindingRead = true; newBinding = "<Mouse>/MiddleButton"; break;

                    // Back
                    case 3: bindingRead = true; newBinding = "<Mouse>/Back"; break;

                    // Forward
                    case 4: bindingRead = true; newBinding = "<Mouse>/Forward"; break;
                }

                if (bindingRead)
                {
                    Reading = false;

                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
            }
        }

        return newBinding;
    }

    public static string ActionBindingFieldLayout(string initialKey, string identifier)
    {
        return ActionBindingField(EditorGUILayout.GetControlRect(), initialKey, identifier);
    }
    
    public static void StopReading()
    {
        Reading = false;
        Identifier = "";
    }
}
