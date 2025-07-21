
using System;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper.Panels
{
    /// <summary> A panel for editing any<see cref="Image"/>. </summary>
    public class ImageEditorPanel : ViewportPanel
    {
        public override string Description => "A panel used for editing any image type.";

        public override string Context { get; internal set; }


        [SerializeField] private List<Type> toolTypes;

        private Dictionary<Type, Tool> tools;

        private List<string> contexts = new()
        {
            HF.Hierarchy.TypeToContext<Hierarchy.Viewport.DrawImage.Context>(),
            //HF.Hierarchy.TypeToContext<Hierarchy.Viewport.MeshImage.Context>(),
        };


        private void Start()
        {
            //contextMenu = new ContextMenu();

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
