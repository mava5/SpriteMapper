
namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {

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
}
