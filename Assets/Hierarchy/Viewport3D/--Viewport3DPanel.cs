
using UnityEngine;


namespace SpriteMapper.Panels.Viewport3D
{
    /// <summary> A viewport panel that uses 3D navigation and visuals. </summary>
    public abstract class Viewport3DPanel : Panel
    {
        [Header("Viewport 3D:")]
        
        

        public Camera Camera { get; private set; }
    }
}
