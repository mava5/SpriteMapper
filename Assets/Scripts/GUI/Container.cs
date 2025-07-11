
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A dockable element that contains two or more dockable elements.
    /// <br/>   Can contain these elements in horizontally or vertically.
    /// </summary>
    public class Container : DockableElement
    {
        public List<DockableElement> Children;
    }
}
