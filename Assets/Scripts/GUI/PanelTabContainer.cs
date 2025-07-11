
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> A dockable element that has a tab for each <see cref="Panel"/> it contains. </summary>
    public class PanelTabContainer : DockableElement
    {
        [SerializeField] private RectTransform topBar;
        [SerializeField] private RectTransform content;

        public List<Panel> Panels { get; private set; } = new();
    }
}
