
namespace SpriteMapper.Actions
{
    public class Flip : Action, IUndoable, IUserExecutable
    {
        public UserActionInfo Info { get; set; }


        #region Action ================================================================== Action

        public Flip()
        {
            FlipImage();
            ActionHistory.SaveUndoStep(this);
        }

        #endregion Action


        #region Undo Logic ============================================================== Undo Logic

        public void Undo() { FlipImage(); }

        public void Redo() { FlipImage(); }

        public int GetMemorySize() { return 0; }

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
