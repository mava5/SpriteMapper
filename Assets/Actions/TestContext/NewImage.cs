
namespace SpriteMapper.Actions
{
    public class NewImage : Action, IUndoable, IUserExecutable
    {
        private Image createdImage;

        private ImageType imageType;
        private int width = 0;
        private int height = 0;


        #region Action ================================================================== Action

        public NewImage()
        {
            // TODO: Create a pop-up for putting in values

            imageType = ImageType.Draw;
            width = 512;
            height = 512;

            CreateNewImage();

            ActionHistory.SaveUndoStep(this);
        }

        public NewImage(ImageType imageType, int width, int height)
        {
            this.imageType = imageType;
            this.width = width;
            this.height = height;

            CreateNewImage();

            ActionHistory.SaveUndoStep(this);
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
                case ImageType.Draw: createdImage = App.Data.CreateImage<DrawImage>(width, height); break;
                case ImageType.Mesh: createdImage = App.Data.CreateImage<MeshImage>(width, height); break;
            }
        }

        #endregion
    }
}
