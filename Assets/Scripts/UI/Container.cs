
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A floating or docked area containing at least one <see cref="Panel"/>.
    /// <br/>   Containers can also be slotted into other containers.
    /// </summary>
    public class Container : DockableElement
    {
        public List<DockableElement> Children;
    }
}
