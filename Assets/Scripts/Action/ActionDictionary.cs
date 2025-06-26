
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   All (<see cref="IUserExecutable"/>) actions, that the user can perform have a name and context.
    /// <br/>   This class links the combination of the two to a <see cref="UserActionInfo"/>.
    /// <br/>   These infos can be modified in the Unity Resources file where a singleton of this class resides.
    /// <br/>   Any <see cref="UserActionInfo"/> can be accessed with an indexer.
    /// </summary>
    [CreateAssetMenu(fileName = "UserActionDictionary", menuName = "SO Singletons/UserActionDictionary")]
    public class UserActionDictionary : ScriptableObject
    {
        public static UserActionDictionary Instance { get; private set; }

        /// <summary> List of all action contexts. </summary>
        [field: SerializeField] public List<string> Contexts { get; private set; } = new();
        
        /// <summary> List of all <see cref="IUserExecutable"/> action infos. </summary>
        [field: SerializeField] public List<UserActionInfo> Infos { get; private set; } = new();


        // An actual dictionary created based on keys and values lists
        private Dictionary<(string context, string name), UserActionInfo> data = new();


        #region Indexer =============================================================================================== Indexer

        /// <summary>
        /// <br/>   Returns <see cref="ExecutableActionInfo"/> stored to dictionary with action's context + name key.
        /// <br/>   Using this indexer for the first time initializes the actual dictionary based on serialized lists.
        /// </summary>
        public UserActionInfo this[string context, string name]
        {
            get
            {
                // Initialize dictionary if it's accessed for the first time
                if (data.Count == 0) { InitializeActualDictionary(); }

                if (!data.ContainsKey((context, name)))
                {
                    return null;
                }

                return data[(context, name)];
            }
        }

        #endregion Indexer


        #region Public Methods ======================================================================================== Public Methods

        public void SetContexts(List<string> contexts) { Contexts = contexts; }
        public void SetInfos(List<UserActionInfo> infos) { Infos = infos; }

        #endregion Public Methods


        #region Private Methods ======================================================================================= Private Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstance()
        {
            // Get all ScriptableObjects from Unity Resources
            UserActionDictionary[] assets = Resources.LoadAll<UserActionDictionary>("");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No UserActionDictionary in Unity Resources.");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple UserActionDictionaries in Unity Resources.");
            }
            else { Instance = assets[0]; }

            ControlsInitializer.Initialize();
        }


        private void InitializeActualDictionary()
        {
        //    for (int i = 0; i < Keys.Count; i++)
        //    {
        //        string[] keySplit = Keys[i].Split("/");
        //        data.Add((keySplit[0], keySplit[1]), Values[i]);
        //    }
        }

        #endregion Private Methods
    }


    /// <summary> Contains necessary information for an <see cref="IUserExecutable"/> <see cref="Action"/>. </summary>
    [Serializable]
    public class UserActionInfo
    {
        public string Name = "";
        public string Context = "Unassigned";
        public Shortcut DefaultShortcut = new();

        /// <summary> Tells if the info actually points to an <see cref="Action"/>. </summary>
        public bool PointsToAction = false;


        public UserActionInfo() { }

        public UserActionInfo(string name, string context, Shortcut defaultShortcut)
        { (Name, Context, DefaultShortcut) = (name, context, defaultShortcut); }
    }


    /// <summary> Shortcuts used by user in the application. </summary>
    [Serializable]
    public class Shortcut
    {
        public bool Shift = false;
        public bool Ctrl = false;
        public bool Alt = false;
        public string Binding = "None";


        public Shortcut() { }

        public Shortcut(bool shift, bool ctrl, bool alt, string binding)
        { (Shift, Ctrl, Alt, Binding) = (shift, ctrl, alt, binding); }
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(UserActionDictionary))]
    public class UserActionDictionaryEditor : Editor
    {
        private UserActionDictionary data;

        // Keeps track of each contexts' foldout menu open state
        private static Dictionary<string, bool> foldoutStates = new();

        private static List<string> testContexts = new()
        {
            "Unassigned",
            "Global",
            "DrawImage",
            "Hmm",
            "Woah",
            "Woah2"
        };

        private static List<UserActionInfo> testInfos = new()
        {
            new("Save", "Global", new(false, true, false, "<KeyBoard>/S")),
            new("Flip", "DrawImage", new(false, false, false, "<KeyBoard>/F")),
            new("Draw", "DrawImage", new(false, false, false, "<Mouse>/RightButton")),
            new("Test1", "Hmm", new(false, true, false, "<Mouse>/MiddleButton")),
            new("Test2", "Woah", new(true, false, true, "<Mouse>/MiddleButton")),
        };


        private void OnEnable() { data = (UserActionDictionary)target; }
        private void OnDisable() { EditorInputControls.StopReading(); }

        private string testBinding = KeyCode.None.ToString();


        #region Inspector GUI ========================================================================================= Inspector GUI

        public override void OnInspectorGUI()
        {
            bool valuesChanged = false;

            var infosInContexts = GetActionInfosInContexts();
            UpdateContextFoldoutStates(infosInContexts);
            
            foreach ((string context, List<UserActionInfo> infos) in infosInContexts)
            {
                // Contain whole list of action infos in a box
                GUI.backgroundColor = new(0.1f, 0.1f, 0.1f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;


                if (context == "Unassigned" || context == "Global")
                {
                    DrawUneditableContextField(context);

                    // Create fields for each action info
                    for (int i = 0; i < infos.Count; i++)
                    {
                        infos[i] = HandleActionInfoField(infos[i]);
                    }
                }
                else
                {
                    (bool toBeDeleted, string newContext) = DrawEditableContextTitle(context);

                    // Delete context list and continue to next one
                    if (toBeDeleted)
                    {
                        // TODO: Undo here
                        foreach (UserActionInfo info in infos) { info.Context = "Unassigned"; }
                        testContexts.Remove(context);
                        continue;
                    }

                    // Only continue with new context name if it's unique
                    // TODO: Undo here
                    if (testContexts.Contains(newContext)) { newContext = context; }
                    else { testContexts[testContexts.IndexOf(context)] = newContext; }

                    // Create fields for each action info
                    for (int i = 0; i < infos.Count; i++)
                    {
                        infos[i].Context = newContext;
                        infos[i] = HandleActionInfoField(infos[i]);
                    }
                }


                // End action infos list
                GUILayout.EndVertical();
            }

            //foreach ((string context, List<UserActionInfo> actions) in infosInContexts)
            //{
            //    (foldoutStates[i], currentContext) = BeginContextList(context, foldoutStates[i]);

            //    GUILayout.EndVertical();
            //}

            //for (int i = 0; i < testValues.Count; i++)
            //{
            //    UserActionInfo info = testValues[i];

            //    string name = info.Name;
            //    string context = info.Context;
            //    Shortcut shortcut = info.DefaultShortcut;

            //    // Context changed -> begin new context's action list
            //    if (info.Context != currentContext)
            //    {


            //    }


            //    // End last context list begun in BeginContextList() start
            //    if (i == testValues.Count - 1) { GUILayout.EndVertical(); }
            //}

            return;

            for (int i = 0; i < 100; i++)
            {
            }

            //Rect lineRect = new(startRect) { width = 1, height = 101 * 27 };
            
            //lineRect.x -= 20;
            //lineRect.y -= 3;

            //EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor);
            //lineRect.x += 1;
            //EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor * 0.75f);
            //lineRect.x += 3;
            //EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor);
            //lineRect.x += 1;
            //EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor * 0.75f);



            //// Draw foldout menus for each action info
            //for (int i = 0; i < data.Keys.Count; i++)
            //{
            //    string[] keySplit = data.Keys[i].Split("/");
            //    string context = keySplit[0], name = keySplit[1];
            //    UserActionInfo info = data.Values[i];


            //    GUILayout.Space(8);

            //    // Toggle context foldout menu, stop here if foldout menu was closed
            //    foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], context);
            //    if (!foldoutStates[i]) { GUILayout.EndVertical(); continue; }


            //    //GUI.backgroundColor = new(0.5f, 0.5f, 0.5f);
            //    //GUILayout.BeginVertical("box");
            //    //GUI.backgroundColor = Color.white;

            //    //Undo.RecordObject(data, "ActionInfo: Shortcut changed");
            //    //data.Values[i].DefaultShortcut = EditorGUILayout.TextField("Shortcut", info.DefaultShortcut);


            //    //Undo.RecordObject(data, "BDD: icon changed");
            //    //EditorGUILayout.BeginHorizontal();
            //    //GUILayout.Label(factionName + " icon");
            //    //data.BDValues[keyIndex].Icon = (Sprite)EditorGUILayout.ObjectField(
            //    //    data.BDValues[keyIndex].Icon, typeof(Sprite), true, GUILayout.Width(64), GUILayout.Height(64));
            //    //EditorGUILayout.EndHorizontal();

            //}

            GUILayout.EndVertical();


            // Set action dictionary dirty for the changes to be saved properly
            if (valuesChanged) { EditorUtility.SetDirty(data); }
        }

        #endregion Inspector GUI


        #region Private Methods ======================================================================================= Private Methods

        /// <summary>
        /// <br/>   Returns dictionary with each context paired with its respective actions.
        /// <br/>   Always contains "Unassigned" and "Global" contexts even if they're empty.
        /// </summary>
        private Dictionary<string, List<UserActionInfo>> GetActionInfosInContexts()
        {
            Dictionary<string, List<UserActionInfo>> infosInContexts = new()
            {
                { "Unassigned", new() },
                { "Global", new() },
            };

            // Try to add already existing contexts to the dictionary
            // TryAdd is used as Unassigned and Global contexts may already exist
            foreach (string context in testContexts)
            {
                infosInContexts.TryAdd(context, new());
            }


            // Create a list of all the UserActionInfos, both pre-existing and new
            // These are then at the end added to their respective contexts
            Dictionary<string, UserActionInfo> infosByName = new();

            // 1. Add pre-existing infos, assume that it doesn't point to an Action
            foreach (UserActionInfo info in testInfos)
            {
                info.PointsToAction = false;
                infosByName.Add(info.Name, info);
            }

            // 2. Add infos for each valid Action, if there isn't a pre-existing one already
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Only check for player executable actions
                if (!type.IsClass || type.Namespace != "SpriteMapper.Actions"
                    || !typeof(IUserExecutable).IsAssignableFrom(type)) { continue; }

                // Skip action if it has a pre-existing info
                // Now we know that the info points to an Action
                if (infosByName.TryGetValue(type.Name, out UserActionInfo info))
                {
                    info.PointsToAction = true;
                    continue;
                }

                // Create new UserActionInfo and add action to Unassigned context
                infosByName.Add(type.Name, new() { Name = type.Name });
            }


            // Now we can match the action infos to their corresponding contexts
            foreach ((string _, UserActionInfo info) in infosByName)
            {
                infosInContexts[info.Context].Add(info);
            }

            return infosInContexts;
        }

        /// <summary> Updates each context's foldout menu state based on context name. </summary>
        private void UpdateContextFoldoutStates(Dictionary<string, List<UserActionInfo>> infosInContexts)
        {
            Dictionary<string, bool> newContextFoldoutStates = new();

            foreach ((string context, List<UserActionInfo> _) in infosInContexts)
            {
                if (foldoutStates.TryGetValue(context, out bool state))
                {
                    newContextFoldoutStates[context] = state;
                }
                else
                {
                    newContextFoldoutStates[context] = false;
                }
            }

            foldoutStates = new(newContextFoldoutStates);
        }


        /// <summary> Draws title label for a given context. </summary>
        private void DrawUneditableContextField(string context)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Offset context title to make room for foldout arrow button
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(14));

            GUILayout.Label(context, GUILayout.Height(20), GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));

            // Create foldout with just the arrow button
            Rect foldoutRect = GUILayoutUtility.GetLastRect();
            foldoutRect.x -= 4;
            foldoutStates[context] = EditorGUI.Foldout(foldoutRect, foldoutStates[context], "");

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// <br/>   Draws title text field and delete button for given context.
        /// <br/>   Returns new context name and boolean which tells if list was deleted.
        /// </summary>
        private (bool, string) DrawEditableContextTitle(string context)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            bool toBeDeleted = false;

            // Offset context title to make room for foldout arrow button
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(14));

            // Get rect for context title by creating an empty label
            GUILayout.Label("", GUILayout.Height(20), GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
            Rect titleRect = GUILayoutUtility.GetLastRect();
            titleRect.width -= 21;

            context = EditorGUI.TextField(titleRect, context);

            // Create a delete button at the right of the text field
            titleRect.x += titleRect.width + 2;
            titleRect.width = 20;
            GUI.backgroundColor = Color.HSVToRGB(0, 0.8f, 1);
            if (GUI.Button(titleRect, "X")) { toBeDeleted = true; }
            GUI.backgroundColor = Color.white;

            // Create foldout with just the arrow button
            Rect foldoutRect = GUILayoutUtility.GetLastRect();
            foldoutRect.x -= 4;
            foldoutStates[context] = EditorGUI.Foldout(foldoutRect, foldoutStates[context], "");

            GUILayout.EndHorizontal();

            return (toBeDeleted, context);
        }


        /// <summary> Handles the drawing and modification of a given action info. </summary>
        private UserActionInfo HandleActionInfoField(UserActionInfo info)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.Label("|| " + info.Name, GUILayout.Width(100));

            // Stop here if info doesn't point to any existing action
            if (!info.PointsToAction)
            {
                GUILayout.Label($"No action found", GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
                GUILayout.EndHorizontal();
                return info;
            }


            ref Shortcut shortcut = ref info.DefaultShortcut;
            // TODO: Undo here
            shortcut.Shift = GUILayout.Toggle(shortcut.Shift, "S", "Button", GUILayout.Width(20));
            shortcut.Ctrl = GUILayout.Toggle(shortcut.Ctrl, "C", "Button", GUILayout.Width(20));
            shortcut.Alt = GUILayout.Toggle(shortcut.Alt, "A", "Button", GUILayout.Width(20));

            GUILayout.Label("", GUILayout.ExpandWidth(true));
            Rect keyRect = GUILayoutUtility.GetLastRect();
            keyRect.height = 19;
            keyRect.width -= 4;
            keyRect.x += 4;

            string identifier = info.Name + info.Context;
            if (EditorInputControls.Reading && identifier == EditorInputControls.Identifier)
            {
                Rect focusRect = new(keyRect) { width = 1 };
                focusRect.x -= 5;
                EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 1));
                focusRect.x += 1;
                EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 0.75f));
            }
            // TODO: Undo here
            shortcut.Binding = EditorInputControls.ActionBindingField(keyRect, shortcut.Binding, identifier);

            GUILayout.EndHorizontal();

            return info;
        }

        #endregion Private Methods
    }

#endif
}