
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
                if (Project != null && Project.Panel != null)
                {
                    if (Project.Action.ActiveContextOverwritingLongAction != null)
                    {
                        return ((LongActionSettings)Project.Action.ActiveContextOverwritingLongAction.Info.Settings).ContextUsedWhenActive;
                    }

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
        private static void InitializeApp()
        {
            // DEBUG
            Project = new();
            Project.Enter();


            HierarchyInfoDictionary.Initialize();
            Controls.Initialize();

            UpdateCaller.SubscribeUpdateCallback(Update);
        }

        #endregion


        #region Update Loop ======================================================================= Update Loop

        private static void Update()
        {
            // Wait until a project is open
            if (Project == null) { return; }


            Actions.UpdateLongActions();

            Actions.ProcessUnresolvedInputs();
            Actions.ProcessQueue();
            
            Controls.UpdateModifierKeys();
        }

        #endregion Update Loop


        #region Public Methods ==================================================================== Public Methods

        public static void OpenProject(Project projectToOpen)
        {
            
        }

        #endregion Public Methods
    }
}
