
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains each open <see cref="SpriteMapper.Project"/> and information about application.
    /// <br/>   Also contains <see cref="ActionHandler"/> as all projects have the same control scheme.
    /// </summary>
    public static class App
    {
        public static List<Project> OpenProjects { get; private set; } = new();
        
        /// <summary> Currently open <see cref="SpriteMapper.Project"/>. </summary>
        public static Project Project { get; private set; }

        public static readonly ActionHandler Controls = new();


        #region App Initialization ================================================================ App Initialization

        /// <summary> Sets up application before first scene loads. </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeApp()
        {
            HierarchyInfoDictionary.Initialize();
            //Controls.Initialize();

            UpdateCaller.SubscribeUpdateCallback(Update);
        }

        #endregion


        #region Update Loop ======================================================================= Update Loop

        private static void Update()
        {
            Controls.Update();
            if (Project != null) { Project.Update(); }
        }

        #endregion Update Loop


        #region Public Methods ==================================================================== Public Methods

        public static void OpenProject(Project projectToOpen)
        {

        }

        #endregion Public Methods
    }
}
