
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

        /// <summary>
        /// <br/>   Contains an <see cref="ActionInfo"/> for each <see cref="Action"/>.
        /// <br/>   Edited in Inspector with <see cref="ActionInfoDictionaryEditor"/>.
        /// <br/>   Setter only works in Unity Editor.
        /// </summary>
        public List<ActionInfo> ActionInfos
        {
            get => actionInfos;

            set { if (Application.isEditor) { actionInfos = value; } } 
        }


        [SerializeField] private List<ActionInfo> actionInfos = new();

        // An actual dictionary created based on keys and values lists
        private Dictionary<Type, ActionInfo> data = new();


        #region Indexer =============================================================================================== Indexer

        /// <summary> Returns a <see cref="ActionInfo"/> for given <see cref="Action"/> type. </summary>
        public ActionInfo this[Type actionType]
        {
            get
            {
                // Initialize data dictionary if it's accessed for the first time
                if (data.Count == 0)
                {
                    foreach (ActionInfo info in actionInfos)
                    {
                        data.Add(Type.GetType(info.ActionFullName), info);
                    }
                }

                if (!data.ContainsKey(actionType)) { return null; }

                return data[actionType];
            }
        }

        #endregion Indexer


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

        #endregion Private Methods
    }


    /// <summary> Contains necessary information for any action. </summary>
    [Serializable]
    public class ActionInfo
    {
        /// <summary> The context, in which action can be used. </summary>
        [field: SerializeField] public Context Context { get; private set; } = Context.Unassigned;

        /// <summary> For example "Save". </summary>
        [field: SerializeField] public string ActionName { get; private set; } = "";

        /// <summary> For example "SpriteMapper.Actions.Global.Save" </summary>
        [field: SerializeField] public string ActionFullName { get; private set; } = "";

        /// <summary> Explanation for how the action works, used for action's tooltip. </summary>
        [field: SerializeField] public string Description { get; private set; } = "";

        [field: SerializeField] public bool IsLong { get; private set; } = false;
        [field: SerializeField] public bool IsUndoable { get; private set; } = false;
        [field: SerializeField] public bool IsUserExecutable { get; private set; } = false;

        /// <summary> Shortcut for executing an <see cref="IUserExecutable"/> action. </summary>
        [field: SerializeField] public Shortcut Shortcut { get; private set; } = null;

        [SerializeField] public bool PointsToAnAction = false;


        /// <summary> Rebinds the action's shortcut. </summary>
        public void Rebind(Shortcut newShortcut)
        { if (IsUserExecutable) { Shortcut = newShortcut; } }


        public void SetIdentifiers(Context context, string actionFullName, string actionName)
        { (Context, ActionFullName, ActionName) = (context, actionFullName, actionName); }

        public void SetDescription(string description)
        { Description = description; }

        public void SetModifiers(bool isLong, bool isUndoable, bool isUserExecutable)
        {
            (IsLong, IsUndoable, IsUserExecutable) = (isLong, isUndoable, isUserExecutable);

            // Remove shortcut from actions that don't need it
            // Also make sure the action has a shortcut if it needs one
            if (!IsUserExecutable) { Shortcut = null; }
            else if (Shortcut == null) { Shortcut = new(); }
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

        // Keeps track of each context's foldout menu open state
        private static Dictionary<Context, bool> menuStates = new();

        private static ActionInfo selectedInfo = null;

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

        private GUIStyle customPadding;
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
            
            customPadding = new();
            customPadding.padding = paddingRect;

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

            GUILayout.BeginVertical(customPadding);
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

            // Update ActionInfo list so that:
            //
            // 1. Pre-existing ActionInfos point to correct Action types.
            // 2. Pre-existing ActionInfos that no longer point to Action types have their action names removed.
            //    → The ActionInfos won't be removed so the user can transfer their information to another
            //      ActionInfo, that actually points to an existing Action type.
            // 3. New ActionInfos are created for those Action types, that don't have ones yet.
            //
            UpdateActionInfoListContents();

            // Sort ActionInfo list by following priority:
            //
            // 1. Context, actionInfos with an invalid context will be moved to Unassigned context.
            // 2. IsUserExecutable, first user executable actions, then the rest.
            // 3. Alphabetically based on ActionName.
            //
            data.ActionInfos = data.ActionInfos.
            /* 1 */ OrderBy(info => validContexts.Contains(info.Context) ? info.Context : Context.Unassigned).
            /* 3 */ ThenBy(info => !info.IsUserExecutable).
            /* 2 */ ThenBy(info => info.ActionName).ToList();
            

            int infoIndex = 0;

            int scrollViewHeight = height - TOP_BAR_HEIGHT - (selectedInfo != null ? PROPERTIES_HEIGHT : 0);
            scrollViewHeight = Mathf.Max(scrollViewHeight, MIN_SCROLL_VIEW_HEIGHT);


            scroll = GUILayout.BeginScrollView(scroll, scrollView, GUILayout.Width(width), GUILayout.Height(scrollViewHeight));
            {
                // Draw Context foldout menus
                foreach (Context context in validContexts)
                {
                    menuStates[context] = HandleCustomFoldout(menuStates[context], context.ToString());

                    // Go through all ActionInfos in current valid context
                    ActionInfo info;
                    while (infoIndex < data.ActionInfos.Count && (info = data.ActionInfos[infoIndex]).Context == context)
                    {
                        // Don't draw Context foldout menu's contents if it's closed
                        if (!menuStates[context]) { infoIndex++; continue; }


                        if ((info.IsUserExecutable && showUserActions) || (!info.IsUserExecutable && showNonUserActions))
                        {
                            GUILayout.Space(S_PAD);
                            bool remove;
                            (remove, data.ActionInfos[infoIndex]) = HandleActionInfoField(info, 20);
                        }
                        infoIndex++;
                    }

                    GUILayout.Space(PAD);
                }
            }
            GUILayout.EndScrollView();
        }

        private void UpdateActionInfoListContents()
        {
            // Organize pre-existing ActionInfos into a dictionary based on their stored ActionFullNames
            // This helps speed up matching Action types found via reflection to already existing ActionInfos
            Dictionary<string, ActionInfo> actionInfosByFullName = new();
            
            foreach (ActionInfo info in data.ActionInfos)
            {
                actionInfosByFullName[info.ActionFullName] = info;

                // Assume that info doesn't point to an action
                // Later when iterating through Action types with reflection, we can reassign the boolean
                // This way the ActionInfos that don't point to any Action type will be easily differentiated
                info.PointsToAnAction = false;
            }

            // Go through each Action type and add their infos to the list
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Only check for action classes
                if (!type.IsClass || string.IsNullOrEmpty(type.Namespace) ||
                    !type.Namespace.StartsWith("SpriteMapper.Actions")) { continue; }


                // Use pre-existing action info if there is one
                ActionInfo actionInfo;
                if (!actionInfosByFullName.TryGetValue(type.FullName, out actionInfo)) { actionInfo = new(); }

                Context context = type.HasAttribute<ActionContext>() ?
                    type.GetCustomAttribute<ActionContext>().Context : Context.Unassigned;

                bool isLong = typeof(ILong).IsAssignableFrom(type);
                bool isUndoable = typeof(IUndoable).IsAssignableFrom(type);
                bool isUserExecutable = typeof(IUserExecutable).IsAssignableFrom(type);

                actionInfo.PointsToAnAction = true;
                actionInfo.SetIdentifiers(context, type.FullName, type.Name);
                actionInfo.SetModifiers(isLong, isUndoable, isUserExecutable);

                actionInfosByFullName[type.FullName] = actionInfo;
            }

            data.ActionInfos = actionInfosByFullName.Values.ToList();
        }

        /// <summary>
        /// <br/>   Makes sure each valid context has a foldout menu state.
        /// <br/>   Uses previously saved state or defaults to false.
        /// </summary>
        private void UpdateFoldoutStates(List<Context> validContexts)
        {
            Dictionary<Context, bool> newStates = new();
            validContexts.ForEach(c => newStates[c] = menuStates.ContainsKey(c) ? menuStates[c] : false);
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

        /// <summary> Handles the drawing and modification of a given action info. </summary>
        private (bool remove, ActionInfo info) HandleActionInfoField(ActionInfo info, int pixelIndentation)
        {
            bool remove = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUIStyle.none, GUILayout.Width(pixelIndentation));
            {
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
                        info.Rebind(newShortcut);

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