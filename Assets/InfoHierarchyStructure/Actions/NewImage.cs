
using UnityEngine;

namespace SpriteMapper.Actions
{
    [InstantActionSettings(false, false, ActionShortcutState.Rebindable, ActionDescendantUsability.Limited)]
    public class NewImage : ShortAction, IUndoable
    {
        private Image createdImage;

        private ImageType imageType;
        private int width = 0;
        private int height = 0;


        #region Action ================================================================== Action

        public override bool Do()
        {
            Debug.Log(Info.Type.Name);

            return true;

            // TODO: Create a pop-up for putting in values

            imageType = ImageType.Draw;
            width = 512;
            height = 512;

            CreateNewImage();

            //App.OpenedProject.History.SaveUndoStep(this);

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
                case ImageType.Draw: createdImage = App.OpenedProject.Data.CreateImage<DrawImage>(width, height); break;
                case ImageType.Mesh: createdImage = App.OpenedProject.Data.CreateImage<MeshImage>(width, height); break;
            }
        }

        #endregion
    }
}
