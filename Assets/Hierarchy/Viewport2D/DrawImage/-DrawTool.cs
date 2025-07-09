
using UnityEngine;


namespace SpriteMapper.Tools.Viewport2D.DrawImage
{
    public class DrawTool : Tool
    {


    }

    public class EquipDrawTool : Action, IShort
    {
        public bool Do() { App.Project.Panel.EquipTool<DrawTool>(); return true; }
    }


    namespace DrawToolEquiped
    {
        public class Draw : Action, ILong, IUndoable
        {
            public bool ShortcutReleased { get; set; }

            public bool EndPredicate => ShortcutReleased;

            public bool CancelPredicate => throw new System.NotImplementedException();


            public bool Begin()
            {
                throw new System.NotImplementedException();
            }

            public void Update()
            {
                throw new System.NotImplementedException();
            }

            public void Cancel()
            {
                throw new System.NotImplementedException();
            }

            public void End()
            {
                throw new System.NotImplementedException();
            }

            public void Redo()
            {
                throw new System.NotImplementedException();
            }

            public void Undo()
            {
                throw new System.NotImplementedException();
            }
        }
    }

    namespace DrawToolActive
    {

    }
}
