
using UnityEngine;


namespace SpriteMapper.Actions
{
    public class NewImage : Action, IUndoable, IUserExecutable
    {
        public UserActionInfo Info { get; set; }


        private Image createdImage;


        #region Action ================================================================== Action

        public NewImage(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Draw: createdImage = new DrawImage(); break;
            }

            ActionHistory.SaveUndoStep(this);
        }

        #endregion Action


        #region Undo Logic ============================================================== Undo Logic

        public void Undo() { }

        public void Redo() { }

        public int GetMemorySize() { return 0; }

        #endregion Undo Logic
    }
}
