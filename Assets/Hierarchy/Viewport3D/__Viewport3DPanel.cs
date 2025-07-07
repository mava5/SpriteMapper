
using UnityEngine;


namespace SpriteMapper.Panels
{
    /// <summary> A viewport panel that uses 3D navigation and visuals. </summary>
    public class Viewport3DPanel : Panel
    {
        [field: Header("Viewport 3D:")]
        [field: SerializeField] public Camera Camera { get; private set; }
    }
}
