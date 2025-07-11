
using UnityEngine;


namespace SpriteMapper
{

    public class ContextMenu
    {
        public bool IsOpen { get; private set; }


        public void Open()
        {
            IsOpen = !IsOpen;
        }
    }
}
