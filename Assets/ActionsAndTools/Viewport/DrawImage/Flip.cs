
namespace SpriteMapper.Actions
{
    [ActionUsedIn(typeof(Context.Viewport.DrawImage))]
    public class Flip : Action, IUndoable
    {
        #region Action ================================================================== Action

        public Flip()
        {
            FlipImage();
            App.Project.History.SaveUndoStep(this);
        }

        #endregion Action


        #region Undo Logic ============================================================== Undo Logic

        public void Undo() { FlipImage(); }

        public void Redo() { FlipImage(); }

        #endregion Undo Logic


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
