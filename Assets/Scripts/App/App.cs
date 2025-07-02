
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Main class for managing the application.
    /// <br/>   Contains and manages different types of handlers.
    /// <br/>   These handlers in turn take care of different parts of the application.
    /// </summary>
    public class App : MonoBehaviour
    {
        public static App Instance { get; private set; }

        public static GUIHandler GUI => Instance.gui;
        public static DataHandler Data => Instance.data;
        public static ActionHandler Action => Instance.action;
        public static ContextHandler Context => Instance.context;
        public static ControlsHandler Controls => Instance.controls;
        public static SelectionHandler Selection => Instance.selection;


        private GUIHandler gui;
        private DataHandler data;
        private ActionHandler action;
        private ContextHandler context;
        private ControlsHandler controls;
        private SelectionHandler selection;


        #region Initialization ========================================================== Initialization

        private void Start()
        {
            Instance = this;

            gui = new GUIHandler();
            data = new DataHandler();
            action = new ActionHandler();
            context = new ContextHandler();
            controls = new ControlsHandler();
            selection = new SelectionHandler();
        }

        #endregion Initialization


        #region Update Loop ============================================================= Update Loop

        private void Update()
        {
            Controls.Update();
        }

        #endregion Update Loop
    }
}
