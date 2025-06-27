
using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.Linq;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   This class contains an <see cref="ActionInfo"/> for each <see cref="Action"/>.
    /// <br/>   The values within this info class can be modified in this ScriptableObject's inspector.
    /// <br/>   These info classes are then linked to each action based on their full name.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionInfoDictionary", menuName = "SO Singletons/ActionInfoDictionary")]
    public class ActionInfoDictionary : ScriptableObject
    {
        public static ActionInfoDictionary Instance { get; private set; }

        // Note: As dictionaries cannot be serialized easily, two lists are used instead.
        //       One list is for the keys of the dictionary and the other for its values.
        //       Here each Action's FullName (key) is linked to an ActionInfo (value).

        // Note: Types cannot be easily serialized, so FullNames are used instead.
        //       Type.GetType(FullName) can then be used to get the Action's type.

        /// <summary> The <see cref="Type.FullName"/> for each <see cref="Action"/>. </summary>
        [field: SerializeField] public List<string> ActionFullNames { get; private set; } = new();
        
        /// <summary> The default <see cref="ActionInfo"/> for each <see cref="Action"/>. </summary>
        [field: SerializeField] public List<ActionInfo> ActionInfos { get; private set; } = new();


        // An actual dictionary created based on keys and values lists
        private Dictionary<Type, ActionInfo> data = new();


        #region Indexer =============================================================================================== Indexer

        /// <summary> Returns a <see cref="ActionInfo"/> for given <see cref="Action"/> type. </summary>
        public ActionInfo this[Type actionType]
        {
            get
            {
                // Initialize dictionary if it's accessed for the first time
                if (data.Count == 0) { InitializeActualDictionary(); }

                if (!data.ContainsKey(actionType)) { return null; }

                return data[actionType];
            }
        }

        #endregion Indexer


        #region Public Methods ======================================================================================== Public Methods

        public void SetActionFullNames(List<string> actionFullNames) { ActionFullNames = actionFullNames; }
        public void SetActionInfos(List<ActionInfo> actionInfos) { ActionInfos = actionInfos; }

        #endregion Public Methods


        #region Private Methods ======================================================================================= Private Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstance()
        {
            // Get all ScriptableObjects from Unity Resources
            ActionInfoDictionary[] assets = Resources.LoadAll<ActionInfoDictionary>("");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No ActionInfoDictionary in Unity Resources.");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple ActionInfoDictionary in Unity Resources.");
            }
            else { Instance = assets[0]; }

            //ControlsInitializer.Initialize(data);
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


    /// <summary> Contains necessary information for any action. </summary>
    [Serializable]
    public class ActionInfo
    {
        /// <summary> The context, in which action can be used. </summary>
        public Context Context { get; private set; } = Context.Unassigned;

        /// <summary> For example "Save". </summary>
        public string ActionName { get; private set; } = "";

        /// <summary> For example "SpriteMapper.Actions.Global.Save" </summary>
        public string ActionFullName { get; private set; } = "";

        /// <summary> Explanation for how the action works, used for action's tooltip. </summary>
        public string Description { get; private set; }

        /// <summary> Shortcut for executing an <see cref="IUserExecutable"/> action. </summary>
        public Shortcut Shortcut { get; private set; } = null;

        public bool IsLong { get; private set; } = false;
        public bool IsUndoable { get; private set; } = false;
        public bool IsUserExecutable { get; private set; } = false;


        public ActionInfo(Context context, string actionFullName, string actionName)
        { (Context, ActionFullName, ActionName) = (context, actionFullName, actionName); }


        /// <summary> Rebinds the action's shortcut. </summary>
        public void Rebind(Shortcut newShortcut) { if (IsUserExecutable) { Shortcut = newShortcut; } }

        /// <summary> Used by <see cref="ActionInfoDictionaryEditor"/>. </summary>
        public void SetModifiers(bool isLong, bool isUndoable, bool isUserExecutable)
        {
            (IsLong, IsUndoable, IsUserExecutable) = (isLong, isUndoable, isUserExecutable);
            Shortcut = (isUserExecutable ? new() : null);
        }
    }


    /// <summary> Shortcut used by an <see cref="IUserExecutable"/> <see cref="Action"/>. </summary>
    [Serializable]
    public class Shortcut
    {
        public bool Shift = false;
        public bool Ctrl = false;
        public bool Alt = false;
        public bool Cmd = false;
        public string Binding = "";

        /// <summary> User readable view of shortcut. </summary>
        public string View =>
            (Shift ? "Shift + " : "") +
            (Ctrl ? "Ctrl + " : "") +
            (Alt ? "Alt + " : "") +
            (Cmd ? "Cmd + " : "") +
            (Binding.Contains("/") ? Binding.Split("/")[1] : "");

        /// <summary> Input System readable binding. </summary>
        // Code from: https://stackoverflow.com/a/21755933
        public string ActionBinding =>
            string.IsNullOrEmpty(Binding) ?
                "" :
                (Binding.Length == 1 ?
                    char.ToLower(Binding[0]).ToString() :
                    char.ToLower(Binding[0]) + Binding[1..]);
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(ActionInfoDictionary))]
    public class ActionInfoDictionaryEditor : Editor
    {
        private ActionInfoDictionary data;

        // Keeps track of each contexts' foldout menu open state
        private static Dictionary<Context, bool> foldoutStates = new();

        private static List<string> testActionFullNames = new();


        private void OnEnable() { data = (ActionInfoDictionary)target; }
        private void OnDisable() { EditorInputControls.StopReadingShortcut(); }


        #region Inspector GUI ========================================================================================= Inspector GUI

        public override void OnInspectorGUI()
        {
            bool valuesChanged = false;

            var actionInfosInContexts = GetActionInfosInContexts();
            UpdateContextFoldoutStates(actionInfosInContexts);

            List<ActionInfo> updatedInfos = new();

            foreach ((Context context, List<ActionInfo> infos) in actionInfosInContexts)
            {
                // Contain whole list of action infos in a box list
                GUI.backgroundColor = new(0.1f, 0.1f, 0.1f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;

                DrawContextFoldoutTitle(context);

                // Create fields for each action info
                for (int i = 0; i < infos.Count; i++)
                {
                    infos[i] = HandleActionInfoField(infos[i]);
                    updatedInfos.Add(infos[i]);
                }

                // End this context's action info list
                GUILayout.EndVertical();
            }

            data.SetActionInfos(updatedInfos);

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
        /// <br/>   Returns dictionary with each context paired with its respective action info.
        /// <br/>   Actions (class) and contexts (namespace) are iterated through via reflection.
        /// <br/>   If a pre-existing action info doesn't point to an action, it's removed.
        /// <br/>   New action infos are made for actions that don't have pre-existing ones.
        /// </summary>
        private Dictionary<Context, List<ActionInfo>> GetActionInfosInContexts()
        {
            Dictionary<Context, List<ActionInfo>> actionInfosInContexts = new();

            // Fill dictionary with valid contexts, invalid pre-existing infos go to Unassigned context
            List<Context> validContexts = ((Context[])Enum.GetValues(typeof(Context))).ToList();
            validContexts.ForEach(context => actionInfosInContexts[context] = new());

            // Organize each pre-existing action info to a list based on the action's name
            // This helps speed up matching actions found via reflection to already existing action infos
            Dictionary<string, ActionInfo> actionInfosByFullName = new();
            data.ActionInfos.ForEach(ai => actionInfosByFullName[ai.ActionFullName] = ai);


            // Go through each Action type and add them to the dictionary
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Only check for action classes
                if (!type.IsClass || string.IsNullOrEmpty(type.Namespace) ||
                    !type.Namespace.StartsWith("SpriteMapper.Actions")) { continue; }


                // Use pre-existing action info if there is one
                ActionInfo actionInfo;
                if (!actionInfosByFullName.TryGetValue(type.FullName, out actionInfo))
                {
                    actionInfo = new(Context.Unassigned, type.FullName, type.Name);
                }

                bool isLong = typeof(ILong).IsAssignableFrom(type);
                bool isUndoable = typeof(IUndoable).IsAssignableFrom(type);
                bool isUserExecutable = typeof(IUserExecutable).IsAssignableFrom(type);

                actionInfo.SetModifiers(isLong, isUndoable, isUserExecutable);

                // Assign action to a pre-defined context if it's valid
                if (validContexts.Contains(actionInfo.Context))
                {
                    actionInfosInContexts[actionInfo.Context].Add(actionInfo);
                }
                else
                {
                    actionInfosInContexts[Context.Unassigned].Add(actionInfo);
                }
            }

            return actionInfosInContexts;
        }

        /// <summary> Updates each context's foldout menu state based on context name. </summary>
        private void UpdateContextFoldoutStates(Dictionary<Context, List<ActionInfo>> infosInContexts)
        {
            Dictionary<Context, bool> newStates = new();

            // Use pre-existing states, default to false if there is no matching one for a context
            foreach ((Context context, List<ActionInfo> _) in infosInContexts)
            {
                newStates[context] = foldoutStates.ContainsKey(context) && foldoutStates[context];
            }

            foldoutStates = new(newStates);
        }


        /// <summary> Draws title label for a given context. </summary>
        private void DrawContextFoldoutTitle(Context context)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Offset context title to make room for foldout arrow button
            GUILayout.Label("", GUILayout.Width(14));

            GUILayout.Label(context.ToString(), GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));

            // Create foldout with just the arrow button
            Rect foldoutRect = GUILayoutUtility.GetLastRect();
            foldoutRect.x -= 4;
            foldoutStates[context] = EditorGUI.Foldout(foldoutRect, foldoutStates[context], "");

            GUILayout.EndHorizontal();
        }


        /// <summary> Handles the drawing and modification of a given action info. </summary>
        private ActionInfo HandleActionInfoField(ActionInfo actionInfo)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.Label(actionInfo.IsLong ? "L" : "", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(10));
            GUILayout.Label(actionInfo.IsUndoable ? "U" : "", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(10));
            GUILayout.Label(new GUIContent(actionInfo.ActionName, "Test"), GUILayout.Width(100), GUILayout.Height(20));

            if (actionInfo.IsUserExecutable)
            {
                GUILayout.Label("", GUILayout.ExpandWidth(true));
                Rect infoRect = GUILayoutUtility.GetLastRect();
                infoRect.height = 19;

                // TODO: Undo here
                (bool focused, Shortcut newShortcut) = EditorInputControls.ShortcutField(infoRect, actionInfo.Shortcut);
                actionInfo.Rebind(newShortcut);

                if (focused)
                {
                    Rect focusRect = new(infoRect) { width = 1 };
                    focusRect.x -= 5;
                    EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 1));
                    focusRect.x += 1;
                    EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 0.75f));
                }
            }

            GUILayout.EndHorizontal();

            return actionInfo;
        }

        #endregion Private Methods
    }

#endif
}