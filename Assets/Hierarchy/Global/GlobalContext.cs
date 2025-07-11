
using UnityEngine;

namespace SpriteMapper.Context.Global { partial class Context { } }

namespace SpriteMapper.Actions.Global
{
    /// <summary> Opens context menu immediately. </summary>
    [NotShortcutExecutable, ActionPriority(PriorityLevel.Low)]
    public class OpenContextMenuShort : Action, IShort
    {
        public bool Do() { App.Project.Panel.ContextMenu.Open(); return true; }
    }

    /// <summary>
    /// <br/>   Used when current context has a <see cref="ILong"/> <see cref="Action"/> with right click <see cref="Shortcut"/>.
    /// <br/>   This action doesn't hinder other actions with right click shortcuts.
    /// <br/>   It only opens context menu if user lets go of right click within certain time frame.
    /// <br/>   For example prevents context menu opening when rotating camera in a Viewport3D panel.
    /// </summary>
    [NotShortcutExecutable, ActionPriority(PriorityLevel.Low)]
    public class OpenContextMenuLong : Action, ILong
    {
        public bool ShortcutReleased { get; set; }

        public bool CancelPredicate => Time.time > startTime + 0.25f;
        public bool EndPredicate => Input.GetMouseButtonUp(1);


        private float startTime = Time.time;


        public bool Begin() { return false; }

        public void Update() { }

        public void Cancel() { }

        public void End() { App.Project.Panel.ContextMenu.Open(); }
    }


    public class Redo : Action, IShort
    {
        public bool Do() { App.Project.History.Redo(); return true; }
    }

    public class Undo : Action, IShort
    {
        public bool Do() { App.Project.History.Undo(); return true; }
    }


    public class NewImage : Action, IShort, IUndoable
    {
        private Image createdImage;

        private ImageType imageType;
        private int width = 0;
        private int height = 0;


        #region Action ================================================================== Action

        public bool Do()
        {
            // TODO: Create a pop-up for putting in values

            imageType = ImageType.Draw;
            width = 512;
            height = 512;

            CreateNewImage();

            App.Project.History.SaveUndoStep(this);

            return true;
        }

        public override void Dispose() { createdImage.Dispose(); }

        #endregion Action


        #region Undo Logic ============================================================== Undo Logic

        public void Undo() { createdImage.Dispose(); }

        public void Redo() { CreateNewImage(); }

        #endregion Undo Logic


        #region Private Methods ========================================================= Private Methods

        private void CreateNewImage()
        {
            switch (imageType)
            {
                case ImageType.Draw: createdImage = App.Project.Data.CreateImage<DrawImage>(width, height); break;
                case ImageType.Mesh: createdImage = App.Project.Data.CreateImage<MeshImage>(width, height); break;
            }
        }

        #endregion
    }
}

namespace SpriteMapper.Tools.Global { }