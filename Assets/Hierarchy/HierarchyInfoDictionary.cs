
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   This class contains an <see cref="ActionInfo"/> for each <see cref="Action"/>.
    /// <br/>   The values within this info class can be modified in this ScriptableObject's inspector.
    /// <br/>   These info classes are then linked to each action based on their full name.
    /// </summary>
    [CreateAssetMenu(fileName = "HierarchyInfoDictionary", menuName = "SO Singletons/HierarchyInfoDictionary")]
    public class HierarchyInfoDictionary : ScriptableObject
    {
        public static HierarchyInfoDictionary Instance { get; private set; }

        /// <summary> Contains an <see cref="ActionInfo"/> for each <see cref="Action"/>. </summary>
        public static Dictionary<Type, ActionInfo> ActionInfos => Instance.actionInfos;

        /// <summary> Contains an <see cref="PanelInfo"/> for each <see cref="Panel"/>. </summary>
        public static Dictionary<Type, PanelInfo> PanelInfos => Instance.panelInfos;

        /// <summary> Contains an <see cref="ToolInfo"/> for each <see cref="Tool"/>. </summary>
        public static Dictionary<Type, ToolInfo> ToolInfos => Instance.toolInfos;

        /// <summary> Used by <see cref="HierarchyInfoDictionaryEditor"/>. </summary>
        public List<SerializedActionInfo> SerializedActionInfos = new();

        /// <summary> Used by <see cref="HierarchyInfoDictionaryEditor"/>. </summary>
        public List<SerializedPanelInfo> SerializedPanelInfos = new();

        /// <summary> Used by <see cref="HierarchyInfoDictionaryEditor"/>. </summary>
        public List<SerializedToolInfo> SerializedToolInfos = new();


        private Dictionary<Type, ActionInfo> actionInfos = new();
        private Dictionary<Type, PanelInfo> panelInfos = new();
        private Dictionary<Type, ToolInfo> toolInfos = new();


        #region Initialization ======================================================================================== Initialization

        public static void Initialize()
        {
            // Get all ScriptableObjects from Unity Resources
            HierarchyInfoDictionary[] assets = Resources.LoadAll<HierarchyInfoDictionary>("");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No HierarchyInfoDictionary in Unity Resources.");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple ActionInfoDictionaries in Unity Resources.");
            }
            else { Instance = assets[0]; Instance.InitializeData(); }
        }

        private void InitializeData()
        {
            // Initialize data based on serialized infos
            foreach (SerializedActionInfo actionInfo in SerializedActionInfos)
            {
                if (actionInfo.PointsToAnAction)
                {
                    actionInfos.Add(Type.GetType(actionInfo.ActionFullName), new ActionInfo(actionInfo));
                }
            }
            foreach (SerializedPanelInfo panelInfo in SerializedPanelInfos)
            {
                panelInfos.Add(Type.GetType(panelInfo.PanelFullName), new PanelInfo(panelInfo));
            }
            foreach (SerializedToolInfo toolInfo in SerializedToolInfos)
            {
                actionInfos.Add(Type.GetType(toolInfo.ToolFullName), new ToolInfo(toolInfo));
            }
        }

        #endregion Initialization


        #region Public Methods ======================================================================================== Public Methods

        public ActionInfo GetActionInfo<T>() where T : Action { return actionInfos[typeof(T)]; }
        public PanelInfo GetPanelInfo<T>() where T : Panel { return panelInfos[typeof(T)]; }
        public ToolInfo GetToolInfo<T>() where T : Tool { return toolInfos[typeof(T)]; }

        #endregion Public Methods
    }


    /// <summary> Contains information for hierarchy items. </summary>
    [Serializable]
    public class SerializedHierarchyItemInfo
    {
        public int Index = 0;
        public string Context = "";

        public string Name = "";
        public string FullName = "";
        public bool PointsToType = false;

        public string Description = "";
    }

    /// <summary> Contains <see cref="Action"/> specific information for creating a runtime <see cref="ActionInfo"/>. </summary>
    [Serializable]
    public class SerializedActionInfo : SerializedHierarchyItemInfo
    {
        public bool IsLong = false;
        public bool IsUndoable = false;
        public bool IsShortcutExecutable = false;

        public PriorityLevel Priority;

        public Shortcut Shortcut = null;
    }

    /// <summary> Contains <see cref="Panel"/> specific information for creating a runtime <see cref="PanelInfo"/>. </summary>
    [Serializable]
    public class SerializedPanelInfo : SerializedHierarchyItemInfo { }

    /// <summary> Contains <see cref="Tool"/> specific information for creating a runtime <see cref="ToolInfo"/>. </summary>
    [Serializable]
    public class SerializedToolInfo : SerializedHierarchyItemInfo { }



#if UNITY_EDITOR

    [CustomEditor(typeof(HierarchyInfoDictionary))]
    public class HierarchyInfoDictionaryEditor : Editor
    {
        private HierarchyInfoDictionary data;

        // Keeps track of each context's foldout menu open state
        private static Dictionary<string, bool> menuStates = new();

        private static SerializedHierarchyItemInfo selectedInfo = null;

        private static Vector2 scroll = new();

        
        private const int TOP_BAR_HEIGHT = 40;
        private const int PROPERTIES_HEIGHT = 200;
        private const int MIN_SCROLL_VIEW_HEIGHT = 50;

        private const int FIELD_HEIGHT = 20;
        private const int INDENT_WIDTH = 20;
        private const int ORDER_ARROW_WIDTH = 15;

        private const int PAD = 5;
        private const int S_PAD = 3;

        private int width = 0, height = 0;
        private bool guiStylesInitialized = false;

        private GUIStyle normalPadding;
        private GUIStyle smallPadding;
        private GUIStyle scrollView;
        private GUIStyle smallText;
        private GUIStyle button;
        private GUIStyle field;


        private void OnEnable() { data = (HierarchyInfoDictionary)target; }

        private void OnDisable() { EditorInputControls.StopReadingShortcut(); }

        public override bool UseDefaultMargins() { return false; }


        #region Inspector GUI ========================================================================================= Inspector GUI

        public override void OnInspectorGUI()
        {
            if (!guiStylesInitialized)
            {
                guiStylesInitialized = true;

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

        #endregion Inspector GUI


        #region TopBar ================================================================================================ TopBar

        private void HandleTopBarGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(TOP_BAR_HEIGHT - PAD));

            /*showUserActions =*/ GUILayout.Toggle(true, "", button,
                GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD),
                GUILayout.MinWidth(0), GUILayout.MinHeight(0));

            GUILayout.Label("", GUIStyle.none, GUILayout.Width(PAD));

            /*showNonUserActions =*/ GUILayout.Toggle(true, "", button,
                GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD),
                GUILayout.MinWidth(0), GUILayout.MinHeight(0));

            GUILayout.EndHorizontal();

            GUILayout.Space(PAD);
        }

        #endregion TopBar


        #region ScrollView ============================================================================================ ScrollView

        private void HandleScrollViewGUI()
        {
            UpdateSerializedHierarchyItemInfoLists();
            SortSerializedHierarchyItemInfoLists();

            List<string> validContexts = GetContexts();
            UpdateFoldoutStates(validContexts);



            int actionInfoIndex = 0;
            int panelInfoIndex = 0;
            int toolInfoIndex = 0;

            int scrollViewHeight = height - TOP_BAR_HEIGHT - (selectedInfo != null ? PROPERTIES_HEIGHT : 0);
            scrollViewHeight = Mathf.Max(scrollViewHeight, MIN_SCROLL_VIEW_HEIGHT);


            scroll = GUILayout.BeginScrollView(scroll, scrollView, GUILayout.Width(width), GUILayout.Height(scrollViewHeight));
            {
                // Draw Context foldout menus
                foreach (string context in validContexts)
                {
                    menuStates[context] = HandleCustomFoldout(menuStates[context], context);

                    // Go through all SerializedActionInfos in current valid context
                    SerializedActionInfo info;
                    while (infoIndex < data.SerializedActionInfos.Count &&
                        (info = data.SerializedActionInfos[infoIndex]).ActionContext == context)
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
        /// <br/>   Update serialized Action, Panel and Tool lists so that:
        /// <br/>   
        /// <br/>   1. Pre-existing infos point to correct types.
        /// <br/>
        /// <br/>   2. Pre-existing infos that no longer point to existing types are separated with PointsToType.
        /// <br/>   The infos won't be outright removed, so the user can transfer any stored information.
        /// <br/>
        /// <br/>   3. New infos are created for those types, that don't have ones yet.
        /// </summary>
        private void UpdateSerializedHierarchyItemInfoLists()
        {
            // Organize pre-existing serialized infos into dictionaries based on the stored FullNames
            // This helps speed up matching types found via reflection to already existing infos
            Dictionary<string, SerializedActionInfo> actionInfosByFullName = new();
            Dictionary<string, SerializedPanelInfo> panelInfosByFullName = new();
            Dictionary<string, SerializedToolInfo> toolInfosByFullName = new();

            // Assume that infos don't point to a type
            // Later when iterating through types with reflection, we can reassign the boolean
            // This way the infos that don't point to any type will be easily differentiated
            foreach (var i in data.SerializedActionInfos) { actionInfosByFullName[i.FullName] = i; i.PointsToType = false; }
            foreach (var i in data.SerializedPanelInfos) { panelInfosByFullName[i.FullName] = i; i.PointsToType = false; }
            foreach (var i in data.SerializedToolInfos) { toolInfosByFullName[i.FullName] = i; i.PointsToType = false; }


            // Go through each Action, Panel and Tool type and add their infos to the list
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                string name = type.Name;
                string fullName = type.FullName;

                // All Actions, Panels and Tools should be in SpriteMapper namespace
                if (!type.IsClass || string.IsNullOrEmpty(fullName) ||
                    !fullName.StartsWith("SpriteMapper.")) { continue; }


                if (fullName.StartsWith("SpriteMapper.Actions."))
                {
                    if (!actionInfosByFullName.TryGetValue(fullName, out var actionInfo)) { actionInfo = new(); }

                    actionInfo.Context = FullNameToContext(fullName);

                    actionInfo.Name = name;
                    actionInfo.FullName = fullName;
                    actionInfo.PointsToType = true;

                    actionInfo.IsLong = typeof(ILong).IsAssignableFrom(type);
                    actionInfo.IsUndoable = typeof(IUndoable).IsAssignableFrom(type);
                    actionInfo.IsShortcutExecutable = !type.HasAttribute<NotShortcutExecutable>();

                    actionInfo.Priority = type.HasAttribute<ActionPriority>() ?
                        type.GetCustomAttribute<ActionPriority>().Priority : PriorityLevel.Normal;

                    actionInfosByFullName[fullName] = actionInfo;
                }
                else if (fullName.StartsWith("SpriteMapper.Panels."))
                {
                    if (!panelInfosByFullName.TryGetValue(fullName, out var panelInfo)) { panelInfo = new(); }

                    panelInfo.Name = name;
                    panelInfo.FullName = fullName;
                    panelInfo.PointsToType = true;

                    panelInfosByFullName[fullName] = panelInfo;
                }
                else if (fullName.StartsWith("SpriteMapper.Tools."))
                {
                    if (!toolInfosByFullName.TryGetValue(fullName, out var toolInfo)) { toolInfo = new(); }

                    toolInfo.Name = name;
                    toolInfo.FullName = fullName;
                    toolInfo.PointsToType = true;

                    toolInfosByFullName[fullName] = toolInfo;
                }
            }

            data.SerializedActionInfos = actionInfosByFullName.Values.ToList();
            data.SerializedPanelInfos = panelInfosByFullName.Values.ToList();
            data.SerializedToolInfos = toolInfosByFullName.Values.ToList();
        }

        /// <summary>
        /// <br/>   Sorts serialized Action, Panel and Tool lists by following priority:
        /// <br/>   1 Context (shortened namespace of hierarchy item)
        /// <br/>   2 [Action only] IsShortcutExecutable
        /// <br/>   3 Index (hierarchy item's order number in a context)
        /// <br/>   Also removes gaps and duplicates from hierarchy items' indices.
        /// </summary>
        private void SortSerializedHierarchyItemInfoLists()
        {
            data.SerializedActionInfos = data.SerializedActionInfos.
            /* 1 */ OrderBy(info => info.Context).
            /* 2 */ ThenBy(info => !info.IsShortcutExecutable).
            /* 3 */ ThenBy(info => info.Index).ToList();

            data.SerializedPanelInfos = data.SerializedPanelInfos.
            /* 1 */ OrderBy(info => info.Context).
            /* 3 */ ThenBy(info => info.Index).ToList();

            data.SerializedToolInfos = data.SerializedToolInfos.
            /* 1 */ OrderBy(info => info.Context).
            /* 3 */ ThenBy(info => info.Index).ToList();


            // Note: Index is divisible by 2 so that when its order can be changed more easily.
            //       For example you can simply add 3 to an action's index so it goes forward once.
            //       This way all following actions' indices don't have to be updated.

            // Remove gaps and duplicate indices
            int currentIndex = 0; string currentContext = "";
            foreach (SerializedActionInfo actionInfo in data.SerializedActionInfos)
            {
                if (actionInfo.Context != currentContext)
                { currentIndex = 0; currentContext = actionInfo.Context; }

                actionInfo.Index = currentIndex += 2;
            }

            currentIndex = 0; currentContext = "";
            foreach (SerializedPanelInfo panelInfo in data.SerializedPanelInfos)
            {
                if (panelInfo.Context != currentContext)
                { currentIndex = 0; currentContext = panelInfo.Context; }

                panelInfo.Index = currentIndex += 2;
            }

            currentIndex = 0; currentContext = "";
            foreach (SerializedToolInfo toolInfo in data.SerializedToolInfos)
            {
                if (toolInfo.Context != currentContext)
                { currentIndex = 0; currentContext = toolInfo.Context; }

                toolInfo.Index = currentIndex += 2;
            }
        }

        /// <summary>
        /// <br/>   Makes sure each valid context has a foldout menu state.
        /// <br/>   Uses previously saved state or defaults to false.
        /// </summary>
        private void UpdateFoldoutStates(List<string> validContexts)
        {
            Dictionary<string, bool> newStates = new();
            validContexts.ForEach(c => newStates[c] = menuStates.ContainsKey(c) ? menuStates[c] : true);
            menuStates = newStates;
        }

        #endregion ScrollView


        #region Properties ============================================================================================ Properties


        #endregion Properties GUI


        #region Private Methods ======================================================================================= Private Methods

        /// <summary>
        /// <br/>   Returns a hierarchy item type's FullName as a context name.
        /// <br/>   For example: "SpriteMapper.Actions.Global.Undo" -> "Global"
        /// </summary>
        private string FullNameToContext(string fullName)
        {
            // For example:
            // X "Actions.Global.Undo"
            // X "SpriteMapper.Actions"
            // X "SpriteMapper.Global.Undo"
            // ✓ "SpriteMapper.Actions.Global.Undo"
            if (!fullName.StartsWith("SpriteMapper.Actions.") ||
                !fullName.StartsWith("SpriteMapper.Panels.") ||
                !fullName.StartsWith("SpriteMapper.Tools."))
            {
                Debug.LogWarning($"Hierarchy item {fullName} has invalid namespace!");
                return "ERROR";
            }

            return fullName[fullName["SpriteMapper.".Length..].IndexOf(".")..fullName.LastIndexOf(".")];
        }


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