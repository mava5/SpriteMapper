
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains each open <see cref="SpriteMapper.Project"/> and information about application.
    /// <br/>   Also contains <see cref="ActionHandler"/> and <see cref="ControlsHandler"/> for processing user input.
    /// </summary>
    public static class App
    {
        public static List<Project> OpenProjects { get; private set; } = new();

        /// <summary> Currently open <see cref="SpriteMapper.Project"/>. </summary>
        public static Project Project { get; private set; } = null;

        public static bool IsProjectOpen => Project != null;

        public static string CurrentContext
        {
            get
            {
                if (Project == null) { return ""; }

                if (Actions.ActiveContextOverwritingLongAction != null)
                {
                    return ((LongActionSettings)Actions.ActiveContextOverwritingLongAction.Info.Settings).ContextUsedWhenActive;
                }

                return "Viewport.ImageEditor.DrawImage";

                if (Project.Panel != null)
                {
                    if (Project.Panel.Tool != null)
                    {
                        return Project.Panel.Tool.Info.Context;
                    }
                    return Project.Panel.Context;
                }
                return "";
            }
        }

        public static readonly ActionHandler Actions = new();
        public static readonly ControlsHandler Controls = new();


        #region App Initialization ================================================================ App Initialization

        /// <summary> Sets up application before first scene loads. </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartAppInitialization()
        {
            MonoBehaviourCaller.Instance.StartCoroutine(InitializeApp());

            MonoBehaviourCaller.Instance.UpdateCallback += Update;
        }

        private static IEnumerator InitializeApp()
        {
            // DEBUG
            Project = new();
            Project.Enter();

            // For some reason loading the HierarchyInfo ScriptableObject from Resources
            // doesn't load its contents properly right away.
            HierarchyInfo hierarchyInfo = Resources.Load<HierarchyInfo>("HierarchyInfo");
            while (true)
            {
                int nonInstantCount = 0;

                foreach (SerializedActionInfo info in hierarchyInfo.SerializedActionInfos)
                {
                    Debug.Log(info.FullName);
                    Debug.Log(info.Settings.Behaviour);
                    Debug.Log(info.Settings.ShortcutState);
                    Debug.Log(info.Settings.DescendantUsability);
                }

                if (hierarchyInfo.SerializedActionInfos[0].Settings.DescendantUsability != ActionDescendantUsability.None) { break; }

                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("TEST");
            //hierarchyInfo.InitializeData();

            yield break;
        }

        #endregion


        #region Update Loop ======================================================================= Update Loop

        private static void Update()
        {
            //// HierarchyInfo's serialized data might not be present yet when app is initialized
            //// That's why data is fetched in update loop until it has properly loaded
            //if (!hierarchyInfoFetched)
            //{
            //    Resources.Load("HierarchyInfo");

            //    foreach (SerializedActionInfo info in HierarchyInfo.Instance.SerializedActionInfos)
            //    {
            //        Debug.LogWarning(info.Settings.DescendantUsability);
            //    }


            //    HierarchyInfo.Instance.InitializeData();
            //    Controls.Initialize();

            //    return;
            //}

            // Wait until a project is open
            if (Project == null) { return; }


            Actions.UpdateLongActions();

            Actions.ProcessUnresolvedInputs();
            Actions.ProcessQueue();
            
            Controls.UpdateVariables();
        }

        #endregion Update Loop


        #region Public Methods ==================================================================== Public Methods

        public static void OpenProject(Project projectToOpen)
        {
            
        }

        #endregion Public Methods
    }
}
