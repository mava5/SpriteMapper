
using System;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper.Panels.Viewport3D.MeshImage
{

    public class MeshImagePanel : Viewport3DPanel
    {
        [SerializeField] private List<Type> toolTypes;

        private Dictionary<Type, Tool> tools;


        private void Start()
        {
            Info = HierarchyInfoDictionary.PanelInfos[typeof(MeshImagePanel)];

            contextMenu = new ContextMenu();

            //foreach (Type toolType in toolTypes)
            //{
            //    tools.Add(toolType, (Tool)Activator.CreateInstance(toolType));
            //}
        }




        public override void EquipTool<T>()
        {
            Tool = tools[typeof(T)];
        }
    }
}
