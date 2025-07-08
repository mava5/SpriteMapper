
using UnityEngine;


namespace SpriteMapper.Actions.Viewport2D.DrawImage
{
    public class DrawTool : Action, ILong, IUndoable
    {
        public bool EndPredicate
        {
            get
            {
                return Input.GetMouseButtonUp(0);
            }
        }

        public bool CancelPredicate
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
        }


        private float brushRadius = 10f;
        private int canvasSize = 200;

        private float[,] newValues;
        private float[,] oldValues;


        #region Action ================================================================== Action

        public bool Begin()
        {
            newValues = new float[canvasSize, canvasSize];
            oldValues = new float[canvasSize, canvasSize];

            return true;
        }

        public void Update()
        {
            int radiusCeiled = Mathf.CeilToInt(brushRadius);

            Vector2 mousePos = Input.mousePosition;
            mousePos -= new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) / 2f;
            mousePos += new Vector2(canvasSize, canvasSize) / 2f;

            int xMouse = Mathf.FloorToInt(mousePos.x), yMouse = Mathf.FloorToInt(mousePos.y);

            for (int xOff = -radiusCeiled; xOff <= radiusCeiled; xOff++)
            {
                for (int yOff = -radiusCeiled; yOff <= radiusCeiled; yOff++)
                {
                    int x = xOff + xMouse;
                    int y = yOff + yMouse;

                    if (x < 0 || x >= canvasSize || y < 0 || y >= canvasSize) { continue; }

                    // Ranges [0, 1], 0 = Center, 1 = Edge
                    float alpha = (mousePos - new Vector2(x + 0.5f, y + 0.5f)).magnitude / brushRadius;
                    alpha = Mathf.Clamp01(alpha);

                    if (Input.GetKey(KeyCode.LeftControl)) { alpha = Mathf.Min(newValues[x, y], alpha); }
                    else { alpha = Mathf.Max(newValues[x, y], 1 - alpha); }

                    newValues[x, y] = alpha;
                }
            }
        }

        public void Cancel()
        {

        }

        public void End()
        {
            //for (int x = 0; x < canvasSize; x++)
            //{
            //    for (int y = 0; y < canvasSize; y++)
            //    {
            //        oldValues[x, y] = Image.canvas[x, y];
            //    }
            //}

            ApplyNewValues();
            App.Project.History.SaveUndoStep(this);
        }

        #endregion Action


        #region Undo Logic ============================================================== Undo Logic

        public void Undo()
        {
            ApplyOldValues();
        }

        public void Redo()
        {
            ApplyNewValues();
        }

        #endregion Undo Logic


        #region Private Methods ========================================================= Private Methods

        private void ApplyNewValues()
        {
            //for (int x = 0; x < canvasSize; x++)
            //{
            //    for (int y = 0; y < canvasSize; y++)
            //    {
            //        float newValue = newValues[x, y];

            //        if (newValue == 0) { continue; }

            //        Image.canvas[x, y] = newValue;
            //    }
            //}
        }

        private void ApplyOldValues()
        {
            //for (int x = 0; x < canvasSize; x++)
            //{
            //    for (int y = 0; y < canvasSize; y++)
            //    {
            //        Image.canvas[x, y] = oldValues[x, y];
            //    }
            //}
        }

        #endregion Private Methods
    }
}
