
using System;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper.Panels.Viewport2D.DrawImage
{

    public class DrawImagePanel : Viewport2DPanel
    {
        [SerializeField] private List<Type> toolTypes;

        private Dictionary<Type, Tool> tools;


        private void Start()
        {
            contextMenu = new ContextMenu();

            foreach (Type toolType in toolTypes)
            {
                tools.Add(toolType, (Tool)Activator.CreateInstance(toolType));
            }
        }


        

        public override void EquipTool<T>()
        {
            Tool = tools[typeof(T)];
        }
    }
}
