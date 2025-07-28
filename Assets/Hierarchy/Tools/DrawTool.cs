
//using UnityEngine;


//namespace SpriteMapper.Hierarchy.Viewport.DrawImage
//{
//    partial class Context
//    {
//        public class EquipDrawTool : Action, IShort
//        {
//            public bool Do() { App.Project.Panel.EquipTool<DrawTool>(); return true; }
//        }

//        public class DrawTool : Tool
//        {

//        }
//    }
//}

//namespace SpriteMapper.Hierarchy.Viewport.DrawImage.DrawToolEquipped
//{
//    partial class Context
//    {
//        public class Draw : Action, ILong, IUndoable
//        {
//            public bool ShortcutReleased { get; set; }

//            public bool EndPredicate => ShortcutReleased;

//            public bool CancelPredicate => false;


//            public bool Begin() { return true; }

//            public void Update() { }

//            public void Cancel() { }

//            public void End() { }


//            public void Redo() { }

//            public void Undo() { }
//        }
//    }
//}