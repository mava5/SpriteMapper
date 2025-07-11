
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains and manages handlers unique to each open project.
    /// <br/>   These handlers in turn abstractify their respective parts of the project.
    /// </summary>
    public class Project
    {
        /// <summary> Currently focused <see cref="SpriteMapper.Panel"/>. </summary>
        public Panel Panel { get; private set; } = null;

        public readonly GUIHandler GUI = new();
        public readonly DataHandler Data = new();
        public readonly ActionHandler Action = new();
        public readonly SelectionHandler Selection = new();

        public readonly ActionHistory History = new();


        //#region Update Loop ============================================================= Update Loop

        //public void Update()
        //{
        //    //Controls.Update();
        //}

        //#endregion Update Loop


        #region Public Methods ========================================================== Public Methods

        public void Open() { }

        public void Close() { }


        public void Enter()
        {
            // Load ProjectScene based on project's information


            // DEBUG
            GameObject gameObject = new("Test_Panel");
            //Panel = gameObject.AddComponent<Panels.DrawImagePanel>();
        }

        public void Exit()
        {
            // Stop all active long actions
        }

        #endregion Public Methods
    }
}
