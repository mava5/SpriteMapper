
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   The main building block for creating normal maps.
    /// <br/>   Can contain depth and / or normal data.
    /// </summary>
    public class Image
    {
        public Vector2Int Size { get; internal set; } = new(0, 0);

        public bool HasDepthData => DepthMap != null;
        public bool HasNormalData => NormalMap != null;

        public Texture2D DepthMap { get; internal set; } = null;
        public Texture2D NormalMap { get; internal set; } = null;
        

        // Shared Properties -------------------------------------------------- Shared Properties

        /// <summary>
        /// <br/>   Determines how many steps the range [0, 1] is split into.
        /// <br/>   0 = No stepping
        /// </summary>
        public int Steps { get; set; } = 0;

        /// <summary> Multiplier for the data values. </summary>
        public float Multiplier { get; set; } = 1f;


        // Depth Properties --------------------------------------------------- Depth Properties



        // Normal Properties -------------------------------------------------- Normal Properties

        public bool FlipX { get; set; } = false;
        public bool FlipY { get; set; } = false;
        public bool FlipZ { get; set; } = false;


        #region Public Methods ========================================================== Public Methods

        /// <summary>
        /// <br/>   Returns a rough estimate of the amount of bytes the image takes up.
        /// <br/>   Actual memory size is higher as only biggest data structures are accounted for.
        /// </summary>
        public virtual int GetMemorySize()
        {
            return GetTextureMemorySize();
        }

        #endregion Public Methods


        #region Private Methods ========================================================= Private Methods

        private int GetTextureMemorySize()
        {
            int textureMemorySize = 0;

            if (HasDepthData) { textureMemorySize += Size.x * Size.y * sizeof(float); }
            if (HasNormalData) { textureMemorySize += Size.x * Size.y * sizeof(float) * 3; }
            
            return textureMemorySize;
        }

        #endregion Private Methods
    }
}
