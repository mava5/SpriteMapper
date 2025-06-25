
using UnityEngine;


namespace SpriteMapper.Actions
{
    public class NewImage : Action, IUndoable
    {
        public override ActionContext Context => ActionContext.DrawImage;


        private Image createdImage;


        public NewImage(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Draw: createdImage = new DrawImage(); break;
            }

            ActionHistory.SaveUndoStep(this);
        }


        #region Public Methods ========================================================== Public Methods

        public void Undo() { FlipImage(); }

        public void Redo() { FlipImage(); }

        public int GetMemorySize() { return 0; }

        #endregion Public Methods


        #region Private Methods ========================================================= Private Methods

        private void FlipImage()
        {
        //    float[,] oldValues = (float[,])Image.canvas.Clone();

        //    for (int x = 0; x < Image.canvasSize; x++)
        //    {
        //        for (int y = 0; y < Image.canvasSize; y++)
        //        {
        //            Image.canvas[x, y] = oldValues[Image.canvasSize - 1 - x, y];
        //        }
        //    }
        }

        #endregion Private Methods
    }
}
