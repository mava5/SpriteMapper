
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
                if (actionInfo.PointsToType)
                {
                    actionInfos.Add(Type.GetType(actionInfo.FullName), new ActionInfo(actionInfo));
                }
            }
            foreach (SerializedPanelInfo panelInfo in SerializedPanelInfos)
            {
                if (panelInfo.PointsToType)
                {
                    panelInfos.Add(Type.GetType(panelInfo.FullName), new PanelInfo(panelInfo));
                }
            }
            foreach (SerializedToolInfo toolInfo in SerializedToolInfos)
            {
                if (toolInfo.PointsToType)
                {
                    toolInfos.Add(Type.GetType(toolInfo.FullName), new ToolInfo(toolInfo));
                }
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
        public bool IsShort = false;
        public bool IsUndoable = false;
        public bool IsShortcutExecutable = false;

        public PriorityLevel Priority;

        public Shortcut DefaultShortcut1 = null;
        public Shortcut DefaultShortcut2 = null;
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

        private static bool showActions = true, showPanels = true, showTools = true;

        
        private const int TOP_BAR_HEIGHT = 40;
        private const int PROPERTIES_HEIGHT = 200;
        private const int MIN_SCROLL_VIEW_HEIGHT = 50;

        private const int FIELD_HEIGHT = 20;
        private const int INDENT_WIDTH = 15;
        private const int ORDER_ARROW_WIDTH = 15;

        private const int PAD = 5;
        private const int S_PAD = 3;

        private int width = 0, height = 0;
        private HashSet<string> contexts = new();

        private bool guiStylesInitialized = false;

        private GUIStyle normalPadding;
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
                smallText.alignment = TextAnchor.MiddleCenter;
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
            contexts.Clear();

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
            {
                showPanels = GUILayout.Toggle(showPanels, "Panels", button,
                    GUILayout.Width((width - 2 * PAD) / 3), GUILayout.Height(TOP_BAR_HEIGHT - PAD));

                GUILayout.Label("", GUIStyle.none, GUILayout.Width(PAD));

                showActions = GUILayout.Toggle(showActions, "Actions", button,
                    GUILayout.Width((width - 2 * PAD) / 3), GUILayout.Height(TOP_BAR_HEIGHT - PAD));

                GUILayout.Label("", GUIStyle.none, GUILayout.Width(PAD));

                showTools = GUILayout.Toggle(showTools, "Tools", button,
                    GUILayout.Width((width - 2 * PAD) / 3), GUILayout.Height(TOP_BAR_HEIGHT - PAD));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(PAD);
        }

        #endregion TopBar


        #region ScrollView ============================================================================================ ScrollView

        private void HandleScrollViewGUI()
        {
            UpdateSerializedHierarchyItemInfoLists();
            data.SerializedActionInfos = GetSortedHierarchyItemInfoList(data.SerializedActionInfos);
            data.SerializedPanelInfos = GetSortedHierarchyItemInfoList(data.SerializedPanelInfos);
            data.SerializedToolInfos = GetSortedHierarchyItemInfoList(data.SerializedToolInfos);

            contexts = Enumerable.ToHashSet(
                contexts.OrderBy(context =>
                {
                    return context switch
                    {
                        "Wrong Context" => "",
                        "No Type" => " ",
                        "Global" => "  ",
                        _ => context
                    };
                }));

            UpdateFoldoutStates(contexts);

            int scrollViewHeight = height - TOP_BAR_HEIGHT - (selectedInfo != null ? PROPERTIES_HEIGHT : 0);
            scrollViewHeight = Mathf.Max(scrollViewHeight, MIN_SCROLL_VIEW_HEIGHT);


            string lastClosedContext = "NULL";
            int actionIndex = 0;
            int panelIndex = 0;
            int toolIndex = 0;

            scroll = GUILayout.BeginScrollView(scroll, scrollView, GUILayout.Width(width), GUILayout.Height(scrollViewHeight));
            {
                // Draw Context foldout menus
                foreach (string context in contexts)
                {
                    // Don't open child contexts of a closed context
                    if (context.StartsWith(lastClosedContext)) { continue; }


                    int currentDepth = context.Count(c => c == '.');
                    int pixelIndent = currentDepth * INDENT_WIDTH;
                    string title = currentDepth > 0 ? context[context.LastIndexOf(".")..] : context;

                    menuStates[context] = ContextFoldout(menuStates[context], title, pixelIndent);

                    if (!menuStates[context])
                    {
                        lastClosedContext = context;
                        GUILayout.Space(PAD);

                        while (actionIndex < data.SerializedActionInfos.Count &&
                            data.SerializedActionInfos[actionIndex].Context.StartsWith(lastClosedContext)) { actionIndex++; }
                        
                        while (panelIndex < data.SerializedPanelInfos.Count &&
                            data.SerializedPanelInfos[panelIndex].Context.StartsWith(lastClosedContext)) { panelIndex++; }

                        while (toolIndex < data.SerializedToolInfos.Count &&
                            data.SerializedToolInfos[toolIndex].Context.StartsWith(lastClosedContext)) { toolIndex++; }

                        continue;
                    }


                    // Panels ------------------------------------------------------------------------------- Panels

                    SerializedPanelInfo panelInfo;
                    while (panelIndex < data.SerializedPanelInfos.Count &&
                        (panelInfo = data.SerializedPanelInfos[panelIndex]).Context == context)
                    {
                        if (showPanels)
                        {
                            Undo.RecordObject(data, "HierarchyInfoDictionary updated");
                            data.SerializedPanelInfos[panelIndex] = HierarchyItemInfoField(panelInfo, pixelIndent);
                        }
                        panelIndex++;
                    }


                    // Actions ------------------------------------------------------------------------------ Actions

                    SerializedActionInfo actionInfo;
                    while (actionIndex < data.SerializedActionInfos.Count &&
                        (actionInfo = data.SerializedActionInfos[actionIndex]).Context == context)
                    {
                        if (showActions)
                        {
                            Undo.RecordObject(data, "HierarchyInfoDictionary updated");
                            data.SerializedActionInfos[actionIndex] = ActionInfoField(actionInfo, pixelIndent);
                        }
                        actionIndex++;
                    }


                    // Tools -------------------------------------------------------------------------------- Tools

                    SerializedToolInfo toolInfo;
                    while (toolIndex < data.SerializedToolInfos.Count &&
                        (toolInfo = data.SerializedToolInfos[toolIndex]).Context == context)
                    {
                        if (showTools)
                        {
                            Undo.RecordObject(data, "HierarchyInfoDictionary updated");
                            data.SerializedToolInfos[toolIndex] = HierarchyItemInfoField(toolInfo, pixelIndent);
                        }
                        toolIndex++;
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
            foreach (var i in data.SerializedActionInfos) { actionInfosByFullName[i.FullName] = i; i.PointsToType = false; i.Context = "No Type"; }
            foreach (var i in data.SerializedPanelInfos) { panelInfosByFullName[i.FullName] = i; i.PointsToType = false; i.Context = "No Type"; }
            foreach (var i in data.SerializedToolInfos) { toolInfosByFullName[i.FullName] = i; i.PointsToType = false; i.Context = "No Type"; }


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
                    if (!actionInfosByFullName.TryGetValue(fullName, out SerializedActionInfo actionInfo)) { actionInfo = new(); }

                    actionInfo.Context = FullNameToContext(fullName);
                    actionInfo.Name = name;
                    actionInfo.FullName = fullName;
                    actionInfo.PointsToType = true;

                    actionInfo.IsLong = typeof(ILong).IsAssignableFrom(type);
                    actionInfo.IsShort = typeof(IShort).IsAssignableFrom(type);
                    actionInfo.IsUndoable = typeof(IUndoable).IsAssignableFrom(type);
                    actionInfo.IsShortcutExecutable = !type.HasAttribute<NotShortcutExecutable>();

                    actionInfo.Priority = type.HasAttribute<ActionPriority>() ?
                        type.GetCustomAttribute<ActionPriority>().Priority : PriorityLevel.Normal;

                    actionInfosByFullName[fullName] = actionInfo;
                }
                else if (fullName.StartsWith("SpriteMapper.Panels."))
                {
                    if (!panelInfosByFullName.TryGetValue(fullName, out SerializedPanelInfo panelInfo)) { panelInfo = new(); }

                    panelInfo.Context = FullNameToContext(fullName);
                    panelInfo.Name = name;
                    panelInfo.FullName = fullName;
                    panelInfo.PointsToType = true;

                    panelInfosByFullName[fullName] = panelInfo;
                }
                else if (fullName.StartsWith("SpriteMapper.Tools."))
                {
                    if (!toolInfosByFullName.TryGetValue(fullName, out SerializedToolInfo toolInfo)) { toolInfo = new(); }

                    toolInfo.Context = FullNameToContext(fullName);
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
        /// <br/>   Returns a sorted and cleaned version of given hierarchy item info list.
        /// <br/>   Also updates available contexts list when sorting through different contexts.
        /// </summary>
        private List<T> GetSortedHierarchyItemInfoList<T>(List<T> infos) where T : SerializedHierarchyItemInfo
        {
            infos = infos.
                OrderBy(info =>
                {
                    contexts.Add(info.Context);
                    return info.Context switch
                    {
                        "Wrong Context" => "",
                        "No Type"       => " ",
                        "Global"        => "  ",
                        _ => info.Context
                    };
                }).
                ThenBy(info =>
                {
                    return true switch
                    {
                        true when typeof(SerializedPanelInfo).IsAssignableFrom(info.GetType())  => 0,
                        true when typeof(SerializedActionInfo).IsAssignableFrom(info.GetType()) &&
                        (info as SerializedActionInfo).IsShortcutExecutable                     => 1,
                        true when typeof(SerializedActionInfo).IsAssignableFrom(info.GetType()) => 2,
                        true when typeof(SerializedToolInfo).IsAssignableFrom(info.GetType())   => 3,
                        _ => int.MaxValue
                    };
                }).
                ThenBy(info => info.Index).ToList();

            // Remove duplicates and gaps from hierarchy items' indices
            int currentIndex = 0;
            string currentContext = "";
            foreach (SerializedHierarchyItemInfo info in infos)
            {
                if (info.Context != currentContext)
                { currentIndex = 0; currentContext = info.Context; }

                info.Index = currentIndex += 2;
            }

            return infos;
        }


        /// <summary>
        /// <br/>   Makes sure each context has a foldout menu state.
        /// <br/>   Uses previously saved state or defaults to false.
        /// </summary>
        private void UpdateFoldoutStates(HashSet<string> contexts)
        {
            Dictionary<string, bool> newStates = new();
            
            foreach (string context in contexts)
            {
                newStates[context] = menuStates.ContainsKey(context) ? menuStates[context] : false;
            }

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
            // X "SpriteMapper.Actions.Undo"
            // ✓ "SpriteMapper.Actions.Global.Undo"
            if ((!fullName.StartsWith("SpriteMapper.Actions.") &&
                !fullName.StartsWith("SpriteMapper.Panels.") &&
                !fullName.StartsWith("SpriteMapper.Tools.")) ||
                fullName.Count(c => c == '.') <= 2)
            {
                Debug.LogWarning($"Hierarchy item {fullName} has invalid namespace!");
                return "Wrong Context";
            }

            return fullName[(fullName.IndexOf(".", fullName.IndexOf(".") + 1) + 1)..fullName.LastIndexOf(".")];
        }


        /// <summary> Handles drawing and opening of a context foldout menu. </summary>
        private bool ContextFoldout(bool state, string title, int pixelIndent)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("", GUIStyle.none, GUILayout.Width(pixelIndent));

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


        /// <summary> Handles the drawing and modification of a given SerializedActionInfo. </summary>
        private SerializedActionInfo ActionInfoField(SerializedActionInfo info, int pixelIndent)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("", GUIStyle.none, GUILayout.Width(pixelIndent));

                GUILayout.BeginVertical(GUILayout.Width(ORDER_ARROW_WIDTH));
                {
                    if (GUILayout.Button("Λ", button, GUILayout.Width(ORDER_ARROW_WIDTH),
                        GUILayout.Height(FIELD_HEIGHT / 2 + S_PAD)))
                    {
                        info.Index -= 3;
                    }
                    if (GUILayout.Button("V", button, GUILayout.Width(ORDER_ARROW_WIDTH),
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
                    GUILayout.Label(new GUIContent(
                        info.IsLong ? "L" :
                        info.IsShort ? "S" :
                        "?",
                        info.IsLong ? "Long action" :
                        info.IsShort ? "Short action" :
                        "Action not long or short"), smallText, GUILayout.Width(10));

                    GUILayout.Label(new GUIContent(
                        info.IsUndoable ? "U" : "",
                        info.IsUndoable ? "Undoable action" : ""), smallText, GUILayout.Width(10));


                    SelectableInfoLabel(info);

                    GUILayout.Label("", GUIStyle.none, GUILayout.Height(FIELD_HEIGHT), GUILayout.ExpandWidth(true));
                    Rect rightRect = GUILayoutUtility.GetLastRect();
                    rightRect.height = FIELD_HEIGHT;

                    //if (!info.PointsToType)
                    //{
                    //    rightRect.width -= FIELD_HEIGHT;
                    //    GUI.Label(rightRect, "Action not found");

                    //    rightRect.x += rightRect.width;
                    //    rightRect.width = FIELD_HEIGHT;
                    //    GUI.backgroundColor = new(1.6f, 0.8f, 0.8f);
                    //    if (GUI.Button(rightRect, "X")) { remove = true; }
                    //    GUI.backgroundColor = Color.white;
                    //}
                    if (info.IsShortcutExecutable)
                    {
                        if (info.DefaultShortcut1.IsEmpty && !info.DefaultShortcut2.IsEmpty)
                        {
                            (info.DefaultShortcut1, info.DefaultShortcut2) = (info.DefaultShortcut2, new());
                        }

                        rightRect.width = (rightRect.width - S_PAD) / 2;
                        info.DefaultShortcut1 = EditorInputControls.ShortcutField(rightRect, info.DefaultShortcut1);

                        rightRect.x += rightRect.width + S_PAD;
                        info.DefaultShortcut2 = EditorInputControls.ShortcutField(rightRect, info.DefaultShortcut2);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            return info;
        }

        /// <summary> Handles the drawing and modification of a given SerializedHierarchyItemInfo. </summary>
        private T HierarchyItemInfoField<T>(T info, int pixelIndent) where T : SerializedHierarchyItemInfo
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("", GUIStyle.none, GUILayout.Width(pixelIndent));

                GUILayout.BeginVertical(GUILayout.Width(ORDER_ARROW_WIDTH));
                {
                    if (GUILayout.Button("Λ", button, GUILayout.Width(ORDER_ARROW_WIDTH),
                        GUILayout.Height(FIELD_HEIGHT / 2 + S_PAD)))
                    {
                        info.Index -= 3;
                    }
                    if (GUILayout.Button("V", button, GUILayout.Width(ORDER_ARROW_WIDTH),
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
                    SelectableInfoLabel(info);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            return info;
        }


        /// <summary> Handles drawing and modification of a label that selects info upon clicking. </summary>
        private void SelectableInfoLabel(SerializedHierarchyItemInfo info)
        {
            if (GUILayout.Button(new GUIContent(info.Name, info.Name + ":\n" + info.Description), EditorStyles.label,
                        GUILayout.Width(100), GUILayout.Height(FIELD_HEIGHT), GUILayout.MinWidth(0)))
            {
                if (info != selectedInfo) { selectedInfo = info; }
                else { selectedInfo = null; }
            }
        }

        #endregion Private Methods
    }

#endif
}