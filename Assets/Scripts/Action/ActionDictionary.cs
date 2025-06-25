
using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   All actions have name and context. This class links the combination of the two to a <see cref="ActionInfo"/>.
    /// <br/>   Actions' infos can be modified in the Unity Resources file where a singleton of this class resides.
    /// <br/>   Any <see cref="ActionInfo"/> can be accessed with an indexer.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionDictionary", menuName = "SO Singletons/ActionDictionary")]
    public class ActionDictionary : ScriptableObject
    {
        public static ActionDictionary Instance { get; private set; }

        // Note: Dictionaries cannot be serialized, instead we can serialize two lists separately.
        //       One list is for dictionary's keys and the other is for its values.
        //       These lists are then combined into an actual dictionary when runtime starts.

        // Keys are a combination of action's context and name
        // For example: "Public/Save", "DrawImage/Flip", "LayerView/NewLayer"

        /// <summary> Used by <see cref="ActionDictionaryEditor"/>. Consists of action context + name. </summary>
        [field: SerializeField] public List<string> Keys { get; set; } = new();
        /// <summary> Used by <see cref="ActionDictionaryEditor"/>. Consists of bullet specific data. </summary>
        [field: SerializeField] public List<ActionInfo> Values { get; set; } = new();


        // An actual dictionary created based on keys and values lists
        private Dictionary<(string context, string name), ActionInfo> data = new();


        #region Indexer =================================================================================================== Indexer

        /// <summary>
        /// <br/>   Returns <see cref="ActionInfo"/> stored to dictionary with action's context + name key.
        /// <br/>   Using this indexer for the first time initializes the actual dictionary based on serialized lists.
        /// </summary>
        public ActionInfo this[string context, string name]
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


        #region Private Methods ========================================================================================= Private Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstance()
        {
            // Get all ScriptableObjects from Unity Resources
            ActionDictionary[] assets = Resources.LoadAll<ActionDictionary>("");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No ActionDictionary in Unity Resources.");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple ActionDictionaries in Unity Resources.");
            }
            else { Instance = assets[0]; }

            ControlsInitializer.Initialize();
        }


        private void InitializeActualDictionary()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                string[] keySplit = Keys[i].Split("/");
                data.Add((keySplit[0], keySplit[1]), Values[i]);
            }
        }

        #endregion Private Methods
    }


    /// <summary> Contains necessary information for an <see cref="Action"/>. </summary>
    [Serializable]
    public class ActionInfo
    {
        public string Name;
        public string Context;

        // TODO: Add Shortcut class
        public string DefaultShortcut;
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(ActionDictionary))]
    public class ActionDictionaryEditor : Editor
    {
        private ActionDictionary data;

        // Keeps track of each contexts' foldout menu open state
        private static List<bool> foldoutStates = new();


        private void OnEnable() { data = (ActionDictionary)target; }
        private void OnDisable() { EditorInputControls.StopReading(); }

        private string testBinding = KeyCode.None.ToString();


        #region Inspector GUI ============================================================================================= Inspector GUI

        public override void OnInspectorGUI()
        {
            bool valuesChanged = false;

            GUIStyle keyFieldBorder = GUIStyle.none;
            keyFieldBorder.border = new(1, 1, 1, 1);

            GUI.backgroundColor = new(0.1f, 0.1f, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;


            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField("", GUILayout.Height(20), GUILayout.Width(14));
            Rect startRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.TextField("TEST", GUILayout.Height(20), GUILayout.ExpandWidth(true));

            Rect foldoutRect = new(startRect);
            foldoutRect.x += 14;
            EditorGUI.Foldout(foldoutRect, true, "");

            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < 100; i++)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("|| Test", GUILayout.Width(100));

                GUILayout.Toggle(true, "S", "Button", GUILayout.Width(20));
                GUILayout.Toggle(false, "C", "Button", GUILayout.Width(20));
                GUILayout.Toggle(true, "A", "Button", GUILayout.Width(20));

                GUILayout.Label("", GUILayout.ExpandWidth(true));

                Rect keyRect = GUILayoutUtility.GetLastRect();
                keyRect.height = 19;
                keyRect.width -= 4;
                keyRect.x += 4;

                if (EditorInputControls.Reading && i.ToString() == EditorInputControls.Identifier)
                {
                    Rect focusRect = new(keyRect) { width = 1 };
                    focusRect.x -= 5;
                    EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 1));
                    focusRect.x += 1;
                    EditorGUI.DrawRect(focusRect, Color.HSVToRGB(0, 0.8f, 0.75f));
                }
                testBinding = EditorInputControls.ActionBindingField(keyRect, testBinding, i.ToString());


                //EditorGUILayout.ToggleLeft("S", true, EditorStyles.iconButton, GUILayout.ExpandWidth(false));
                //EditorGUILayout.ToggleLeft("C", true, EditorStyles.iconButton, GUILayout.ExpandWidth(false));
                //EditorGUILayout.ToggleLeft("A", true, EditorStyles.iconButton, GUILayout.ExpandWidth(false));
                //EditorGUILayout.

                EditorGUILayout.EndHorizontal();
            }

            Rect lineRect = new(startRect) { width = 1, height = 101 * 27 };
            
            lineRect.x -= 20;
            lineRect.y -= 3;

            EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor);
            lineRect.x += 1;
            EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor * 0.75f);
            lineRect.x += 3;
            EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor);
            lineRect.x += 1;
            EditorGUI.DrawRect(lineRect, EditorStyles.label.normal.textColor * 0.75f);



            // Draw foldout menus for each action info
            for (int i = 0; i < data.Keys.Count; i++)
            {
                string[] keySplit = data.Keys[i].Split("/");
                string context = keySplit[0], name = keySplit[1];
                ActionInfo info = data.Values[i];


                GUILayout.Space(8);

                // Toggle context foldout menu, stop here if foldout menu was closed
                foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], context);
                if (!foldoutStates[i]) { GUILayout.EndVertical(); continue; }


                //GUI.backgroundColor = new(0.5f, 0.5f, 0.5f);
                //GUILayout.BeginVertical("box");
                //GUI.backgroundColor = Color.white;

                Undo.RecordObject(data, "ActionInfo: Shortcut changed");
                data.Values[i].DefaultShortcut = EditorGUILayout.TextField("Shortcut", info.DefaultShortcut);


                //Undo.RecordObject(data, "BDD: icon changed");
                //EditorGUILayout.BeginHorizontal();
                //GUILayout.Label(factionName + " icon");
                //data.BDValues[keyIndex].Icon = (Sprite)EditorGUILayout.ObjectField(
                //    data.BDValues[keyIndex].Icon, typeof(Sprite), true, GUILayout.Width(64), GUILayout.Height(64));
                //EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndVertical();


            // Set action dictionary dirty for the changes to be saved properly
            if (valuesChanged) { EditorUtility.SetDirty(data); }
        }

        #endregion Inspector GUI


        #region Private Methods ===================================================================== Private Methods

        /// <summary>
        /// <br/>   Updates keys list to track modifications done to <see cref="Temp_BulletType"/> and <see cref="Faction"/>.
        /// <br/>   Updates values list and foldout states list based on changes done to keys list.
        /// </summary>
        //private void UpdateLists()
        //{
        //    Temp_BulletType[] bulletTypes = (Temp_BulletType[])Enum.GetValues(typeof(Temp_BulletType));
        //    Faction[] factions = (Faction[])Enum.GetValues(typeof(Faction));

        //    List<string> updatedKeys = new();
        //    List<BulletData> updatedBDValues = new();
        //    List<BulletTypeData> updatedSharedBDValues = new();


        //    // Note: By storing keys as strings instead of enums,
        //    //       keys will point to correct values even if the enums' orders are changed

        //    // Update keys and values lists based on BulletType enums
        //    for (int typeIndex = 0; typeIndex < bulletTypes.Length; typeIndex++)
        //    {
        //        for (int factionIndex = 0; factionIndex < factions.Length; factionIndex++)
        //        {
        //            // Concatenate BulletType and Faction to form current key
        //            string currentKey = bulletTypes[typeIndex].ToString() + "/" + factions[factionIndex].ToString();

        //            // Check if key is already in the Keys, if it is we can reuse the previously stored value
        //            int previouslyStoredIndex = data.Keys.IndexOf(currentKey);


        //            // Add new value as one doesn't exist for current key yet
        //            if (previouslyStoredIndex == -1 || data.BDValues.Count <= previouslyStoredIndex)
        //            {
        //                updatedKeys.Add(currentKey);
        //                updatedBDValues.Add(new());
        //            }
        //            // Add previously stored value instead of creating new one
        //            else
        //            {
        //                updatedKeys.Add(currentKey);
        //                updatedBDValues.Add(data.BDValues[previouslyStoredIndex]);
        //            }
        //        }

        //        // Create new shared bullet data if one wasn't previously stored
        //        if (data.BTDValues.Count <= typeIndex) { updatedSharedBDValues.Add(null); }
        //        else { updatedSharedBDValues.Add(data.BTDValues[typeIndex]); }
        //    }

        //    data.Keys = updatedKeys;
        //    data.BDValues = updatedBDValues;
        //    data.BTDValues = updatedSharedBDValues;


        //    // Make sure all bullet types have a foldout menu boolean list
        //    int boolListDelta = updatedKeys.Count - foldoutMenuStates.Count;
        //    if (boolListDelta != 0)
        //    {
        //        for (int i = 0; i < Mathf.Abs(boolListDelta); i++)
        //        {
        //            if (boolListDelta > 0)
        //            {
        //                // Add an element for each faction as well as one for whole bullet type foldout menu
        //                foldoutMenuStates.Add(Enumerable.Repeat(false, 1 + factions.Length).ToList());
        //            }
        //            else { foldoutMenuStates.RemoveAt(updatedKeys.Count - i); }
        //        }
        //    }

        //    // Make sure all bullet type foldout menus have enough booleans for factions
        //    for (int i = 0; i < foldoutMenuStates.Count; i++)
        //    {
        //        // +1 for boolean which tells if whole bullet type foldout menu is open
        //        int boolDelta = 1 + factions.Length - foldoutMenuStates[i].Count;

        //        // Discard excess booleans
        //        if (boolDelta < 0) { foldoutMenuStates[i] = foldoutMenuStates[i].Take(1 + factions.Length).ToList(); }

        //        // Add false for each missing boolean
        //        else { foldoutMenuStates[i].AddRange(Enumerable.Repeat(false, boolDelta)); }
        //    }
        //}

        #endregion Private Methods
    }

#endif
}