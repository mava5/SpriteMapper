
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains each loaded <see cref="Project"/> and information about application.
    /// <br/>   Also contains <see cref="ActionHandler"/> and <see cref="ControlsHandler"/> for processing user input.
    /// </summary>
    public class App : MonoBehaviour
    {
        public static App Instance = null;

        public static Hierarchy Hierarchy => Instance.hierarchy;
        public static ActionHandler Actions => Instance.actions;
        public static ControlsHandler Controls => Instance.controls;

        public static Project OpenedProject => Instance.openedProject;
        public static List<Project> LoadedProjects => Instance.loadedProjects;

        public static bool IsProjectOpen => OpenedProject != null;


        private readonly Hierarchy hierarchy = new();
        private readonly ActionHandler actions = new();
        private readonly ControlsHandler controls = new();

        private Project openedProject;
        private List<Project> loadedProjects = new();


        #region App Initialization ================================================================ App Initialization

        /// <summary> Sets up application before first scene loads. </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartApp()
        {
            GameObject gameObject = new("[App]");
            Instance = gameObject.AddComponent<App>();

            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            // DEBUG
            //Project = new();
            //Project.Enter();

            var hierarchyInfo = await Addressables.LoadAssetAsync<HierarchyInfo>("HierarchyInfo").Task;
            hierarchyInfo.InitializeStaticData();
        }

        #endregion


        #region Update Loop ======================================================================= Update Loop

        private void Update()
        {
            //// Wait until a project is open
            //if (Project == null) { return; }


            //Actions.UpdateLongActions();

            //Actions.ProcessUnresolvedInputs();
            //Actions.ProcessQueue();
            
            //Controls.UpdateVariables();
        }

        #endregion Update Loop


        #region Public Methods ==================================================================== Public Methods

        public static void OpenProject(Project projectToOpen)
        {
            
        }

        #endregion Public Methods
    }
}
