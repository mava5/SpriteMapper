
using UnityEngine;

namespace SpriteMapper.Panels
{
    public abstract class ViewportPanel : Panel
    {
        public Camera Camera { get; private set; }
    }
}