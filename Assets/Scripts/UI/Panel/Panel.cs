
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A focusable rectangular area with a given context, actions and tools.
    /// <br/>   Inherits parent and ancestor panels' actions and tools as well.
    /// </summary>
    public abstract class Panel : MonoBehaviour
    {
        /// <summary> Currently equiped <see cref="SpriteMapper.Tool"/>. </summary>
        public Tool Tool { get; internal set; } = null;

        public PanelInfo Info { get; internal set; } = null;


        internal ContextMenu contextMenu = null;


        public void OpenContextMenu()
        {
            contextMenu.Open();
        }


        public abstract void EquipTool<T>() where T : Tool;
    }
}
