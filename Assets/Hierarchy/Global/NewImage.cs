
using UnityEngine;

namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(false, false, ActionShortcutState.Exists, ActionDescendantUsability.Limited)]
        public class NewImage : ShortAction, IUndoable
        {
            private Image createdImage;

            private ImageType imageType;
            private int width = 0;
            private int height = 0;


            #region Action ================================================================== Action

            public override bool Do()
            {
                Debug.Log(Info.ActionType.Name);

                return true;

                // TODO: Create a pop-up for putting in values

                imageType = ImageType.Draw;
                width = 512;
                height = 512;

                CreateNewImage();

                App.Project.History.SaveUndoStep(this);

                return true;
            }

            internal override void Dispose(bool manuallyCalled) { createdImage.Dispose(); }

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
}
