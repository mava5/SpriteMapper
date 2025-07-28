
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   This class contains an <see cref="ActionInfo"/> for each <see cref="Action"/>.
    /// <br/>   The values within this info class can be modified in this ScriptableObject's inspector.
    /// <br/>   These info classes are then linked to each action based on their full name.
    /// </summary>
    [CreateAssetMenu(fileName = "HierarchyInfo", menuName = "SO Singletons/HierarchyInfo")]
    public class HierarchyInfo : ScriptableObject
    {
        public const string INVALID_CONTEXT = "Invalid Context";
        public const string NO_TYPE_CONTEXT = "No Type";

        /// <summary> Contains an <see cref="ToolInfo"/> for each <see cref="Tool"/>. </summary>
        public static Dictionary<Type, ToolInfo> ToolInfos { get; private set; } = new();

        /// <summary> Contains an <see cref="ActionInfo"/> for each <see cref="Action"/>. </summary>
        public static Dictionary<Type, ActionInfo> ActionInfos { get; private set; } = new();

        /// <summary> Used by <see cref="HierarchyInfoEditor"/>. </summary>
        public List<SerializedToolInfo> SerializedToolInfos = new();

        /// <summary> Used by <see cref="HierarchyInfoEditor"/>. </summary>
        public List<SerializedActionInfo> SerializedActionInfos = new();


        #region Initialization ======================================================================================== Initialization

        /// <summary> Initializes static variables with serialized data stored to this object. </summary>
        public void InitializeStaticData()
        {
            ToolInfos.Clear();
            ActionInfos.Clear();

            // Initialize data based on serialized infos
            foreach (SerializedToolInfo toolInfo in SerializedToolInfos)
            {
                if (toolInfo.Context == INVALID_CONTEXT)
                {
                    Debug.LogWarning($"Tool {toolInfo.FullName} has invalid context!");
                    continue;
                }
                if (!toolInfo.PointsToType)
                {
                    Debug.LogWarning($"Tool {toolInfo.FullName} doesn't point to a Tool type!");
                    continue;
                }

                ToolInfos.Add(Type.GetType(toolInfo.FullName), new ToolInfo(toolInfo));
            }
            foreach (SerializedActionInfo actionInfo in SerializedActionInfos)
            {
                Type actionType = Type.GetType(actionInfo.FullName);
                ActionSettings settings = actionType.GetCustomAttribute<ActionSettings>();

                if (actionInfo.Context == INVALID_CONTEXT)
                {
                    Debug.LogWarning($"Action {actionInfo.FullName} has invalid context!");
                    continue;
                }
                if (!actionInfo.PointsToType)
                {
                    Debug.LogWarning($"Action {actionInfo.FullName} doesn't point to an Action type!");
                    continue;
                }
                if (settings == null)
                {
                    Debug.LogWarning($"Action {actionInfo.FullName} has no settings attribute!");
                    continue;
                }

                ActionInfos.Add(actionType, new ActionInfo(actionInfo, settings));
            }
        }

        #endregion Initialization
        

        #region Public Methods ======================================================================================== Public Methods

        /// <summary>
        /// <br/>   Returns a hierarchy item type's FullName as a context name.
        /// <br/>   For example: "SpriteMapper.Hierarchy.Global.Undo" -> "Global"
        /// <br/>   INVALID_CONTEXT string is returned if FullName couldn't be converted.
        /// </summary>
        public static string FullNameToContext(string fullName)
        {
            if (!fullName.StartsWith("SpriteMapper.Hierarchy."))
            {
                Debug.LogWarning($"Hierarchy item {fullName} has invalid namespace!");
                return INVALID_CONTEXT;
            }

            return fullName[(fullName.IndexOf(".", fullName.IndexOf(".") + 1) + 1)..fullName.LastIndexOf(".")];
        }

        public static string TypeToContext<T>() { return FullNameToContext(typeof(T).FullName); }


        //public static MultiPanelInfo GetMultiPanelInfo<T>() where T : MultiPanel { return MultiPanelInfos[typeof(T)]; }
        //public static PanelInfo GetPanelInfo<T>() where T : Panel { return PanelInfos[typeof(T)]; }
        public static ActionInfo GetActionInfo<T>() where T : Action { return ActionInfos[typeof(T)]; }
        public static ToolInfo GetToolInfo<T>() where T : Tool { return ToolInfos[typeof(T)]; }

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

    /// <summary> Contains <see cref="Tool"/> specific information for creating a runtime <see cref="ToolInfo"/>. </summary>
    [Serializable]
    public class SerializedToolInfo : SerializedHierarchyItemInfo { }

    /// <summary> Contains <see cref="Action"/> specific information for creating a runtime <see cref="ActionInfo"/>. </summary>
    [Serializable]
    public class SerializedActionInfo : SerializedHierarchyItemInfo
    {
        public bool IsUndoable = false;
        
        public Shortcut DefaultShortcut = null;
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(HierarchyInfo))]
    public class HierarchyInfoEditor : Editor
    {
        private HierarchyInfo data;

        // Keeps track of each context's foldout menu open state
        private static Dictionary<string, bool> menuStates = new();

        private static SerializedHierarchyItemInfo selectedInfo = null;

        private static Vector2 scroll = new();

        private static bool showTools = true, showActions = true;

        
        private const int TOP_BAR_HEIGHT = 40;
        private const int PROPERTIES_HEIGHT = 200;
        private const int MIN_SCROLL_VIEW_HEIGHT = 50;

        private const int FIELD_HEIGHT = 20;
        private const int INDENT_WIDTH = 15;
        private const int ORDER_ARROW_WIDTH = 15;

        private const int PAD = 5;
        private const int S_PAD = 3;

        private int width = 0, height = 0;
        private HashSet<string> currentContexts = new();
        private Dictionary<string, ActionSettings> currentActionSettings = new();

        private bool guiStylesInitialized = false;

        private GUIStyle normalPadding;
        private GUIStyle scrollView;
        private GUIStyle smallText;
        private GUIStyle button;
        private GUIStyle field;


        private void OnEnable() { data = (HierarchyInfo)target; }

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
            currentContexts.Clear();
            currentActionSettings.Clear();

            GUILayout.BeginVertical(normalPadding);
            {
                HandleTopBarGUI();
                HandleScrollViewGUI();
                //HandlePropertiesGUI();
            }
            GUILayout.EndVertical();

            EditorUtility.SetDirty(data);
        }

        #endregion Inspector GUI


        #region TopBar ================================================================================================ TopBar

        private void HandleTopBarGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(TOP_BAR_HEIGHT - PAD));
            {
                showTools = GUILayout.Toggle(showTools, "Tools", button,
                    GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD));

                GUILayout.Label("", GUIStyle.none, GUILayout.Width(PAD));

                showActions = GUILayout.Toggle(showActions, "Actions", button,
                    GUILayout.Width((width - PAD) / 2), GUILayout.Height(TOP_BAR_HEIGHT - PAD));
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
            data.SerializedToolInfos = GetSortedHierarchyItemInfoList(data.SerializedToolInfos);

            currentContexts = Enumerable.ToHashSet(
                currentContexts.OrderBy(context =>
                {
                    return context switch
                    {
                        HierarchyInfo.INVALID_CONTEXT => "",
                        HierarchyInfo.NO_TYPE_CONTEXT => " ",
                        "Global" => "  ",
                        _ => context
                    };
                }));

            UpdateFoldoutStates(currentContexts);

            int scrollViewHeight = height - TOP_BAR_HEIGHT - (selectedInfo != null ? PROPERTIES_HEIGHT : 0);
            scrollViewHeight = Mathf.Max(scrollViewHeight, MIN_SCROLL_VIEW_HEIGHT);


            string lastClosedContext = "<NULL>";
            int actionIndex = 0;
            int toolIndex = 0;

            Undo.RecordObject(data, "HierarchyInfo updated");

            scroll = GUILayout.BeginScrollView(scroll, scrollView, GUILayout.Width(width), GUILayout.Height(scrollViewHeight));
            {
                // Draw Context foldout menus
                foreach (string context in currentContexts)
                {
                    // Don't open child contexts of a closed context
                    if (context.StartsWith(lastClosedContext)) { continue; }


                    int currentDepth = context.Count(c => c == '.');
                    int pixelIndent = currentDepth * INDENT_WIDTH;
                    string title = currentDepth > 0 ? context[(context.LastIndexOf(".") + 1)..] : context;

                    menuStates[context] = ContextFoldout(menuStates[context], title, pixelIndent);

                    if (!menuStates[context])
                    {
                        lastClosedContext = context;
                        GUILayout.Space(PAD);

                        while (toolIndex < data.SerializedToolInfos.Count &&
                            data.SerializedToolInfos[toolIndex].Context.StartsWith(lastClosedContext)) { toolIndex++; }

                        while (actionIndex < data.SerializedActionInfos.Count &&
                            data.SerializedActionInfos[actionIndex].Context.StartsWith(lastClosedContext)) { actionIndex++; }

                        continue;
                    }


                    // Tools -------------------------------------------------------------------------------- Tools

                    SerializedToolInfo toolInfo;
                    while (toolIndex < data.SerializedToolInfos.Count &&
                        (toolInfo = data.SerializedToolInfos[toolIndex]).Context == context)
                    {
                        if (showTools)
                        {
                            toolInfo = HierarchyItemInfoField(toolInfo, pixelIndent);

                            if (toolInfo == null)
                            {
                                data.SerializedToolInfos.RemoveAt(toolIndex);
                                continue;
                            }

                            data.SerializedToolInfos[toolIndex] = toolInfo;
                        }
                        toolIndex++;
                    }


                    // Actions ------------------------------------------------------------------------------ Actions

                    SerializedActionInfo actionInfo;
                    while (actionIndex < data.SerializedActionInfos.Count &&
                        (actionInfo = data.SerializedActionInfos[actionIndex]).Context == context)
                    {
                        if (showActions)
                        {
                            actionInfo = HierarchyItemInfoField(actionInfo, pixelIndent);

                            if (actionInfo == null)
                            {
                                data.SerializedActionInfos.RemoveAt(actionIndex);
                                continue;
                            }

                            data.SerializedActionInfos[actionIndex] = actionInfo;
                        }
                        actionIndex++;
                    }


                    GUILayout.Space(PAD);
                }
            }
            GUILayout.EndScrollView();
        }


        /// <summary>
        /// <br/>   Update serialized Action and Tool lists so that:
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
            Dictionary<string, SerializedToolInfo> toolInfosByFullName = new();
            Dictionary<string, SerializedActionInfo> actionInfosByFullName = new();

            // Assume that infos don't point to a type
            // Later when iterating through types with reflection, we can reassign the boolean
            // This way the infos that don't point to any type will be easily differentiated
            foreach (SerializedToolInfo toolInfo in data.SerializedToolInfos)
            {
                toolInfosByFullName[toolInfo.FullName] = toolInfo;
                toolInfo.PointsToType = false;
                toolInfo.Context = HierarchyInfo.NO_TYPE_CONTEXT;
            }
            foreach (SerializedActionInfo actionInfo in data.SerializedActionInfos)
            {
                actionInfosByFullName[actionInfo.FullName] = actionInfo;
                actionInfo.PointsToType = false;
                actionInfo.Context = HierarchyInfo.NO_TYPE_CONTEXT;
            }


            // Go through each Action and Tool type and add their infos to the list
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                string name = type.Name;
                string fullName = type.FullName;

                // All Actions and Tools should be in SpriteMapper.Hierarchy namespace
                if (!type.IsClass || string.IsNullOrEmpty(fullName) ||
                    !fullName.StartsWith("SpriteMapper.Hierarchy.")) { continue; }


                if (typeof(Tool).IsAssignableFrom(type))
                {
                    if (!toolInfosByFullName.TryGetValue(fullName, out SerializedToolInfo toolInfo)) { toolInfo = new(); }

                    toolInfo.Context = HierarchyInfo.FullNameToContext(fullName);
                    toolInfo.Name = name;
                    toolInfo.FullName = fullName;
                    toolInfo.PointsToType = true;

                    toolInfosByFullName[fullName] = toolInfo;
                }
                else if (typeof(Action).IsAssignableFrom(type))
                {
                    if (!actionInfosByFullName.TryGetValue(fullName, out SerializedActionInfo actionInfo)) { actionInfo = new(); }

                    actionInfo.Context = HierarchyInfo.FullNameToContext(fullName);
                    actionInfo.Name = name;
                    actionInfo.FullName = fullName;
                    actionInfo.PointsToType = true;

                    actionInfo.IsUndoable = typeof(IUndoable).IsAssignableFrom(type);

                    currentActionSettings.Add(fullName, type.GetCustomAttribute<ActionSettings>());

                    actionInfosByFullName[fullName] = actionInfo;
                }
            }

            data.SerializedActionInfos = actionInfosByFullName.Values.ToList();
            data.SerializedToolInfos = toolInfosByFullName.Values.ToList();
        }


        /// <summary>
        /// <br/>   Returns a sorted and cleaned version of given hierarchy item info list.
        /// <br/>   Also updates available contexts list when sorting through different contexts.
        /// </summary>
        private List<T> GetSortedHierarchyItemInfoList<T>(List<T> infos) where T : SerializedHierarchyItemInfo
        {
            infos = infos.
                // Context
                OrderBy(info =>
                {
                    currentContexts.Add(info.Context);
                    return info.Context switch
                    {
                        HierarchyInfo.INVALID_CONTEXT => "",
                        HierarchyInfo.NO_TYPE_CONTEXT => " ",
                        "Global"        => "  ",
                        _ => info.Context
                    };
                }).
                // First actions (based on shortcut state) then tools
                ThenBy(info =>
                {
                    if (typeof(SerializedToolInfo).IsAssignableFrom(info.GetType()))
                    {
                        return -1;
                    }
                    else if (typeof(SerializedActionInfo).IsAssignableFrom(info.GetType()))
                    {
                        return currentActionSettings[info.FullName]?.ShortcutState switch
                        {
                            ActionShortcutState.Rebindable => 0,
                            ActionShortcutState.Locked => 1,
                            ActionShortcutState.None => 2,
                            _ => 3,
                        };
                    }
                    else { return int.MaxValue; }
                }).
                // Index
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


        #endregion Properties


        #region Private Methods ======================================================================================= Private Methods

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


        /// <summary> Handles the drawing and modification of a given SerializedHierarchyItemInfo. </summary>
        private T HierarchyItemInfoField<T>(T info, int pixelIndent) where T : SerializedHierarchyItemInfo
        {
            bool isTool = typeof(SerializedToolInfo).IsAssignableFrom(typeof(T));
            bool isAction = typeof(SerializedActionInfo).IsAssignableFrom(typeof(T));

            SerializedToolInfo toolInfo = isTool ? info as SerializedToolInfo : null;
            SerializedActionInfo actionInfo = isAction ? info as SerializedActionInfo : null;

            ActionSettings actionSettings = isAction ? currentActionSettings[actionInfo.FullName] : null;

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
                    if (isTool)
                    {
                        GUILayout.Label("", GUIStyle.none, GUILayout.Width(20));

                        SelectableInfoLabel(toolInfo);
                    }
                    else if (isAction)
                    {
                        GUILayout.Label(
                            actionSettings?.Behaviour switch
                            {
                                ActionBehaviourType.Instant => new GUIContent("I", "Instant action [Pressed, Short]"),
                                ActionBehaviourType.Toggle => new GUIContent("T", "Toggle action [Pressed, Long]"),
                                ActionBehaviourType.Hold => new GUIContent("H", "Hold action [Held, Long]"),
                                null => new GUIContent("?", "Action has no settings"),
                                _ => new GUIContent()
                            }, smallText, GUILayout.Width(10));

                        GUILayout.Label(actionInfo.IsUndoable ? new GUIContent("U", "Action can be undone / redone") : new(),
                            smallText, GUILayout.Width(10));

                        SelectableInfoLabel(actionInfo);
                    }


                    GUILayout.Label("", GUIStyle.none, GUILayout.Height(FIELD_HEIGHT), GUILayout.ExpandWidth(true));
                    Rect rightRect = GUILayoutUtility.GetLastRect();
                    rightRect.height = FIELD_HEIGHT;

                    if (!info.PointsToType)
                    {
                        rightRect.width -= FIELD_HEIGHT;
                        GUI.Label(rightRect, "Doesn't point to type");

                        rightRect.x += rightRect.width;
                        rightRect.width = FIELD_HEIGHT;
                        GUI.backgroundColor = new(1.6f, 0.8f, 0.8f);
                        if (GUI.Button(rightRect, "X")) { info = null; }
                        GUI.backgroundColor = Color.white;
                    }
                    else if (isTool)
                    {

                    }
                    else if (isAction)
                    {
                        if (actionSettings == null)
                        {
                            GUI.Label(rightRect, "Action has no settings");
                        }
                        else if (actionSettings.ShortcutState != ActionShortcutState.None)
                        {
                            rightRect.width = rightRect.width;

                            if (actionSettings.ShortcutState == ActionShortcutState.Locked)
                            {
                                Rect lockRect = new(rightRect) { width = 10, x = rightRect.x - 10 - S_PAD };
                                GUI.Label(lockRect, "🔒", smallText);
                            }

                            actionInfo.DefaultShortcut = EditorInputControls.ShortcutField(rightRect, actionInfo.DefaultShortcut);
                        }
                    }
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
                        GUILayout.Width(120), GUILayout.Height(FIELD_HEIGHT), GUILayout.MinWidth(0)))
            {
                if (info != selectedInfo) { selectedInfo = info; }
                else { selectedInfo = null; }
            }
        }

        #endregion Private Methods
    }

#endif
}