
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;


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

        /// <summary> Contains an <see cref="ActionInfo"/> for each <see cref="Action"/>. </summary>
        public static Dictionary<Type, ActionInfo> Data => Instance.data;

        /// <summary>
        /// <br/>   Contains an <see cref="SerializedActionInfo"/> for each <see cref="Action"/>.
        /// <br/>   Edited in Inspector with <see cref="ActionInfoDictionaryEditor"/>.
        /// </summary>
        public List<SerializedActionInfo> SerializedActionInfos = new();


        public Dictionary<Type, ActionInfo> data { get; private set; } = new();


        #region Initialization ======================================================================================== Initialization

        public static void Initialize()
        {
            // Get all ScriptableObjects from Unity Resources
            ActionInfoDictionary[] assets = Resources.LoadAll<ActionInfoDictionary>("");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No ActionInfoDictionary in Unity Resources.");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple ActionInfoDictionaries in Unity Resources.");
            }
            else { Instance = assets[0]; }
        }

        #endregion Initialization


        #region Indexer =============================================================================================== Indexer

        /// <summary> Returns a <see cref="ActionInfo"/> for given <see cref="Action"/> type. </summary>
        public ActionInfo this[Type actionType]
        {
            get
            {
                // Initialize data dictionary if it's accessed for the first time
                if (data.Count == 0)
                {
                    foreach (SerializedActionInfo info in SerializedActionInfos)
                    {
                        data.Add(Type.GetType(info.ActionFullName), new ActionInfo(info));
                    }
                }

                if (!data.ContainsKey(actionType)) { return null; }

                return data[actionType];
            }
        }

        #endregion Indexer
    }


    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    public class ActionInfo
    {
        /// <summary> The context, in which action can be used. </summary>
        public readonly Context Context;

        /// <summary> The type of the action the info points to. </summary>
        public readonly Type ActionType;

        /// <summary> Explanation for how the action works, used for action's tooltip. </summary>
        public readonly string Description;

        public readonly bool IsLong;
        public readonly bool IsUndoable;
        public readonly bool IsUserExecutable;

        /// <summary> Shortcut for executing an <see cref="IUserExecutable"/> action. </summary>
        public Shortcut Shortcut { get; private set; } = null;


        public ActionInfo(SerializedActionInfo serializedInfo)
        {
            Context = serializedInfo.Context;
            ActionType = Type.GetType(serializedInfo.ActionFullName);
            Description = serializedInfo.Description;

            IsLong = serializedInfo.IsLong;
            IsUndoable = serializedInfo.IsUndoable;
            IsUserExecutable = serializedInfo.IsUserExecutable;

            Shortcut = serializedInfo.Shortcut;
        }


        /// <summary> Rebinds the action's shortcut. </summary>
        public void Rebind(Shortcut newShortcut)
        { if (IsUserExecutable) { Shortcut = newShortcut; } }
    }


    /// <summary>
    /// <br/>   Serializes <see cref="Action"/> specific information.
    /// <br/>   Gets turned into a <see cref="ActionInfo"/> in runtime.
    /// </summary>
    [Serializable]
    public class SerializedActionInfo
    {
        public int Index = 0;
        public Context Context = Context.Unassigned;
        
        public bool PointsToAnAction = false;
        public string ActionName = "";
        public string ActionFullName = "";

        public string Description = "";

        public bool IsLong = false;
        public bool IsUndoable = false;
        public bool IsUserExecutable = false;

        public Shortcut Shortcut = null;
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
        // Code help from: https://stackoverflow.com/a/21755933
        public string InputBinding =>
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

        // Keeps track of each context's foldout menu open state
        private static Dictionary<Context, bool> menuStates = new();

        private static SerializedActionInfo selectedInfo = null;

        private static Vector2 scroll = new();

        private static bool showUserActions = true;
        private static bool showNonUserActions = false;
     
        
        private const int TOP_BAR_HEIGHT = 40;
        private const int PROPERTIES_HEIGHT = 200;
        private const int MIN_SCROLL_VIEW_HEIGHT = 50;

        private const int FIELD_HEIGHT = 20;

        private const int PAD = 5;
        private const int S_PAD = 3;

        private int width = 0, height = 0;

        private GUIStyle normalPadding;
        private GUIStyle smallPadding;
        private GUIStyle scrollView;
        private GUIStyle smallText;
        private GUIStyle button;
        private GUIStyle field;


        private void OnEnable()
        {
            data = (ActionInfoDictionary)target;
            
            RectOffset zeroRect = new(0, 0, 0, 0);
            RectOffset paddingRect = new(PAD, PAD, PAD, PAD);
            RectOffset smallPaddingRect = new(S_PAD, S_PAD, S_PAD, S_PAD);
            
            normalPadding = new() { padding = paddingRect };
            smallPadding = new() { padding = smallPaddingRect };

            scrollView = new("LODBlackBox");
            scrollView.padding = paddingRect;
            scrollView.margin = zeroRect;
            scrollView.fixedHeight = 0;
            scrollView.fixedWidth = 0;

            smallText = EditorStyles.centeredGreyMiniLabel;
            smallText.padding = zeroRect;
            smallText.margin = zeroRect;
            smallText.fixedHeight = 0;
            smallText.fixedWidth = 0;
            smallText.normal.textColor = new(0.6f, 0.6f, 0.6f);

            button = EditorStyles.miniButton;
            button.padding = smallPaddingRect;
            button.margin = zeroRect;
            button.fixedHeight = 0;
            button.fixedWidth = 0;

            field = new("FrameBox");
            field.padding = smallPaddingRect;
            field.margin = zeroRect;
            field.fixedHeight = 0;
            field.fixedWidth = 0;
        }

        private void OnDisable() { EditorInputControls.StopReadingShortcut(); }


        #region Inspector GUI ========================================================================================= Inspector GUI

        public override void OnInspectorGUI()
        {
            width = Mathf.Max(Mathf.RoundToInt(EditorGUIUtility.currentViewWidth), 0) - PAD * 2;
            height = Screen.height - 170 - PAD * 2;

            GUILayout.BeginVertical(normalPadding);
            {
                HandleTopBarGUI();
                HandleScrollViewGUI();
                //HandlePropertiesGUI();
            }
            GUILayout.EndVertical();

            EditorUtility.SetDirty(data);
            return;
        }

        // Remove default margins
        public override bool UseDefaultMargins() { return false; }

        #endregion Inspector GUI


        #region TopBar ================================================================================================ TopBar

        private void HandleTopBarGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(TOP_BAR_HEIGHT - PAD));

            showUserActions = GUILayout.Toggle(showUserActions, "Show Executable", button,
                GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD),
                GUILayout.MinWidth(0), GUILayout.MinHeight(0));

            GUILayout.Label("", GUIStyle.none, GUILayout.Width(PAD));

            showNonUserActions = GUILayout.Toggle(showNonUserActions, "Show Non-executable", button,
                GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD),
                GUILayout.MinWidth(0), GUILayout.MinHeight(0));

            GUILayout.EndHorizontal();

            GUILayout.Space(PAD);
        }

        #endregion TopBar


        #region ScrollView ============================================================================================ ScrollView

        private void HandleScrollViewGUI()
        {
            List<Context> validContexts = ((Context[])Enum.GetValues(typeof(Context))).ToList();
            UpdateFoldoutStates(validContexts);

            UpdateSerializedActionInfoListContents();
            SortSerializedActionInfoList(validContexts);
            

            int infoIndex = 0;

            int scrollViewHeight = height - TOP_BAR_HEIGHT - (selectedInfo != null ? PROPERTIES_HEIGHT : 0);
            scrollViewHeight = Mathf.Max(scrollViewHeight, MIN_SCROLL_VIEW_HEIGHT);


            scroll = GUILayout.BeginScrollView(scroll, scrollView, GUILayout.Width(width), GUILayout.Height(scrollViewHeight));
            {
                // Draw Context foldout menus
                foreach (Context context in validContexts)
                {
                    menuStates[context] = HandleCustomFoldout(menuStates[context], context.ToString());

                    // Go through all SerializedActionInfos in current valid context
                    SerializedActionInfo info;
                    while (infoIndex < data.SerializedActionInfos.Count &&
                        (info = data.SerializedActionInfos[infoIndex]).Context == context)
                    {
                        // Don't draw Context foldout menu's contents if it's closed
                        if (!menuStates[context]) { infoIndex++; continue; }


                        if ((info.IsUserExecutable && showUserActions) || (!info.IsUserExecutable && showNonUserActions))
                        {
                            GUILayout.Space(S_PAD);
                            bool remove;
                            (remove, data.SerializedActionInfos[infoIndex]) = HandleActionInfoField(info);
                        }
                        infoIndex++;
                    }

                    GUILayout.Space(PAD);
                }
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// <br/>   Update SerializedActionInfo list so that:
        /// <br/>   
        /// <br/>   1. Pre-existing infos point to correct Action types.
        /// <br/>
        /// <br/>   2. Pre-existing infos that no longer point to Action types have their action names removed.
        /// <br/>   The infos won't be removed so the user can transfer their information
        /// <br/>   to another info, that actually points to an existing Action type.
        /// <br/>
        /// <br/>   3. New infos are created for those Action types, that don't have ones yet.
        /// </summary>
        private void UpdateSerializedActionInfoListContents()
        {
            // Organize pre-existing SerializedActionInfos into a dictionary based on their stored ActionFullNames
            // This helps speed up matching Action types found via reflection to already existing ActionInfos
            Dictionary<string, SerializedActionInfo> infosByFullName = new();
            
            foreach (SerializedActionInfo info in data.SerializedActionInfos)
            {
                infosByFullName[info.ActionFullName] = info;

                // Assume that SerializedActionInfo doesn't point to an Action type
                // Later when iterating through Action types with reflection, we can reassign the boolean
                // This way the SerializedActionInfos that don't point to any Action type will be easily differentiated
                info.PointsToAnAction = false;
            }

            // Go through each Action type and add their infos to the list
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Only check for action classes
                if (!type.IsClass || string.IsNullOrEmpty(type.Namespace) ||
                    !type.Namespace.StartsWith("SpriteMapper.Actions")) { continue; }


                // Use pre-existing SerializedActionInfo if there is one, otherwise create a new one
                if (!infosByFullName.TryGetValue(type.FullName, out SerializedActionInfo info)) { info = new(); }

                info.Context = type.HasAttribute<ActionContext>() ?
                    type.GetCustomAttribute<ActionContext>().Context : Context.Unassigned;

                info.PointsToAnAction = true;
                info.ActionFullName = type.FullName;
                info.ActionName = type.Name;

                info.IsLong = typeof(ILong).IsAssignableFrom(type);
                info.IsUndoable = typeof(IUndoable).IsAssignableFrom(type);
                info.IsUserExecutable = typeof(IUserExecutable).IsAssignableFrom(type);

                infosByFullName[type.FullName] = info;
            }

            data.SerializedActionInfos = infosByFullName.Values.ToList();
        }

        /// <summary>
        /// <br/>   Sorts SerializedActionInfo list by following priority:
        /// <br/>   1. Context; infos with an invalid context will be moved to Unassigned context.
        /// <br/>   2. IsUserExecutable; first user executable actions, then the rest.
        /// <br/>   3. Based on Index or in other words the Action's order number in a Context.
        /// <br/>   Also makes sure that all actions have proper indices in their corresponding contexts.
        /// </summary>
        private void SortSerializedActionInfoList(List<Context> validContexts)
        {
            data.SerializedActionInfos = data.SerializedActionInfos.
            /* 1. */ OrderBy(info => validContexts.Contains(info.Context) ? info.Context : Context.Unassigned).
            /* 2. */ ThenBy(info => !info.IsUserExecutable).
            /* 3. */ ThenBy(info => info.Index).ToList();

            // Remove gaps and duplicate indices
            int currentIndex = 0;
            Context currentContext = Context.Unassigned;
            foreach (SerializedActionInfo info in data.SerializedActionInfos)
            {
                if (info.Context != currentContext)
                {
                    currentIndex = 0;
                    currentContext = info.Context;
                }

                info.Index = currentIndex;

                // Note: Index is divisible by 2 so that when its order can be changed more easily.
                //       For example you can simply add 3 to an action's index so it goes forward once.
                //       This way all following actions' indices don't have to be updated.
                currentIndex += 2;
            }
        }

        /// <summary>
        /// <br/>   Makes sure each valid context has a foldout menu state.
        /// <br/>   Uses previously saved state or defaults to false.
        /// </summary>
        private void UpdateFoldoutStates(List<Context> validContexts)
        {
            Dictionary<Context, bool> newStates = new();
            validContexts.ForEach(c => newStates[c] = menuStates.ContainsKey(c) ? menuStates[c] : true);
            menuStates = newStates;
        }

        #endregion ScrollView


        #region Properties ============================================================================================ Properties


        #endregion Properties GUI


        #region Private Methods ======================================================================================= Private Methods

        /// <summary> Handles a modified foldout menu that better aligns in a HelpBox. </summary>
        private bool HandleCustomFoldout(bool state, string title)
        {
            GUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = new(0.8f, 0.8f, 0.8f);
                GUILayout.BeginHorizontal(field);
                GUI.backgroundColor = Color.white;
                {
                    // Offset title to make room for foldout arrow button
                    GUILayout.Label("", GUILayout.Height(FIELD_HEIGHT), GUILayout.Width(14));

                    GUILayout.Label(title, GUILayout.Height(FIELD_HEIGHT), GUILayout.MinWidth(0));

                    // Create foldout with just the arrow button
                    Rect foldoutRect = GUILayoutUtility.GetLastRect();
                    foldoutRect.x -= 4;
                    state = EditorGUI.Foldout(foldoutRect, state, "", true);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            return state;
        }

        /// <summary> Handles the drawing and modification of a given ActionInfo. </summary>
        private (bool remove, SerializedActionInfo info) HandleActionInfoField(SerializedActionInfo info)
        {
            bool remove = false;

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(FIELD_HEIGHT));
                {
                    if (GUILayout.Button("Λ", button, GUILayout.Width(FIELD_HEIGHT),
                        GUILayout.Height(FIELD_HEIGHT / 2 + S_PAD)))
                    {
                        info.Index -= 3;
                    }
                    if (GUILayout.Button("V", button, GUILayout.Width(FIELD_HEIGHT),
                        GUILayout.Height(FIELD_HEIGHT / 2 + S_PAD)))
                    {
                        info.Index += 3;
                    }
                }
                GUILayout.EndVertical();

                GUI.backgroundColor = Color.white * (info == selectedInfo ? 1.25f : 1);
                GUILayout.BeginHorizontal(field);
                GUI.backgroundColor = Color.white;
                {
                    GUILayout.Label(new GUIContent(info.IsLong ? "L" : "", (info.IsLong ? "Is" : "Not") + " ILong"),
                        smallText, GUILayout.Width(10));
                    GUILayout.Label(new GUIContent(info.IsUndoable ? "U" : "", (info.IsUndoable ? "Is" : "Not") + " IUndoable"),
                        smallText, GUILayout.Width(10));

                    if (GUILayout.Button(new GUIContent(info.ActionName, info.Description), EditorStyles.label,
                        GUILayout.Width(100), GUILayout.Height(FIELD_HEIGHT), GUILayout.MinWidth(0)))
                    {
                        if (info != selectedInfo)   { selectedInfo = info; }
                        else                        { selectedInfo = null; }
                    }


                    GUILayout.Label("", GUIStyle.none, GUILayout.Height(FIELD_HEIGHT), GUILayout.ExpandWidth(true));
                    Rect rightRect = GUILayoutUtility.GetLastRect();
                    rightRect.height = FIELD_HEIGHT;

                    if (!info.PointsToAnAction)
                    {
                        rightRect.width -= FIELD_HEIGHT;
                        GUI.Label(rightRect, "Action not found");

                        rightRect.x += rightRect.width;
                        rightRect.width = FIELD_HEIGHT;
                        GUI.backgroundColor = new(1.6f, 0.8f, 0.8f);
                        if (GUI.Button(rightRect, "X")) { remove = true; }
                        GUI.backgroundColor = Color.white;
                    }
                    else if (info.IsUserExecutable)
                    {
                        // TODO: Undo here
                        (bool focused, Shortcut newShortcut) = EditorInputControls.ShortcutField(rightRect, info.Shortcut);
                        info.Shortcut = newShortcut;

                        if (focused)
                        {
                            rightRect.width = 1;
                            rightRect.x -= 5;
                            EditorGUI.DrawRect(rightRect, Color.HSVToRGB(0, 0.8f, 1));
                            rightRect.x += 1;
                            EditorGUI.DrawRect(rightRect, Color.HSVToRGB(0, 0.8f, 0.75f));
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            return (remove, info);
        }

        #endregion Private Methods
    }

#endif
}