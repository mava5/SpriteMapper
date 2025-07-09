
using System.Collections.Generic;
using System;
using UnityEngine;


namespace SpriteMapper.Panels.Viewport3D.Preview
{

    public class PreviewPanel : Viewport3DPanel
    {

        private void Start()
        {
            contextMenu = new ContextMenu();
        }




        public override void EquipTool<T>() { }
    }
}
