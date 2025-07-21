
using UnityEngine;


namespace SpriteMapper.Hierarchy.Viewport.DrawImage
{
    partial class Context
    {
        public class EquipFlipTool : Action, IShort
        {
            public bool Do() { App.Project.Panel.EquipTool<FlipTool>(); return true; }
        }

        public class FlipTool : Tool
        {

        }
    }
}

namespace SpriteMapper.Hierarchy.Viewport.DrawImage.FlipToolEquipped
{
    partial class Context
    {
        public class FlipVertically : Action, IShort, IUndoable
        {
            public bool Do() { return true; }

            public void Redo() { }

            public void Undo() { }
        }

        public class FlipHorizontally : Action, IShort, IUndoable
        {
            public bool Do() { return true; }

            public void Redo() { }

            public void Undo() { }
        }
    }
}