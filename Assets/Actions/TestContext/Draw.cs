
using UnityEngine;


namespace Actions.DrawImage
{
    public class Draw : Action, ILong, IUndoable
    {
        public bool EndPredicate
        {
            get
            {
                return Input.GetMouseButtonUp(0) && Random.value > 0.5f;
            }
        }

        public bool CancelPredicate
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
        }


        #region Public Functions ================================================== Public Functions

        public void Begin()
        {
            TestInput.AddToUpdateList(this);
            Debug.Log("Start");
        }

        public void Update()
        {

        }

        public void Cancel()
        {
            Debug.Log("Cancel");
        }

        public void End()
        {
            Debug.Log("End");
        }


        public void Undo()
        {

        }

        public void Redo()
        {

        }

        #endregion Public Functions
    }
}
