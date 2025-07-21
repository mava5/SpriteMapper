
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> A dockable element that has a tab for each <see cref="Panel"/> it contains. </summary>
    public class PanelTabContainer : DockableElement
    {
        [SerializeField] private RectTransform tabBarRect;
        [SerializeField] private RectTransform panelsRect;
        [SerializeField] private RectTransform dockAreaRect;
        [SerializeField] private RectTransform outlineRect;

        public List<Panel> Panels { get; private set; } = new();


        #region Public Methods ========================================================== Public Methods


        #endregion Public Methods
    }
}
