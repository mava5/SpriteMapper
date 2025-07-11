
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Handles the project's GUI. </summary>
    public class GUIHandler
    {
        public Container MainContainer { get; private set; } = null;

        public List<Container> FloatingContainers { get; private set; } = new();
    }
}
