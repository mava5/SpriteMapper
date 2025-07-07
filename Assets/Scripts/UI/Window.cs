
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A floating or docked area containing at least one <see cref="Panel"/>.
    /// <br/>   Windows can also be slotted into other windows.
    /// </summary>
    public class Window
    {
        public Window Parent;
        public List<Window> Children;
    }
}
