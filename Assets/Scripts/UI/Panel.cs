
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A focusable rectangular area with a given <see cref="Context"/>.
    /// <br/>   For example the <see cref="Panels.PreviewPanel"/> shows the final result.
    /// <br/>   One or more panels can be slotted into a <see cref="Window"/>.
    /// </summary>
    public class Panel
    {
        public virtual string PanelContext => Context.Default;
    }
}
