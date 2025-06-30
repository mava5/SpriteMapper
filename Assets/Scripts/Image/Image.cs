
using UnityEngine;


namespace SpriteMapper
{
    public enum ImageType
    {
        None,
        Draw,
        Mesh,
    }

    /// <summary>
    /// <br/>   The main building block for creating normal maps.
    /// <br/>   Can contain depth and / or normal data.
    /// </summary>
    public abstract class Image
    {
        public ImageType Type { get; internal set; } = ImageType.None;
        
        public int Width { get; internal set; } = 0;
        public int Height { get; internal set; } = 0;

        public bool HasDepthData { get; internal set; } = false;
        public bool HasNormalData { get; internal set; } = false;

        /// <summary> Low resolution version of the Image for previewing purposes. </summary>
        public Texture2D Preview { get; internal set; } = null;


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


        public Image(int width, int height) { Width = width; Height = height; }


        #region Public Methods ==================================================================== Public Methods

        public abstract Texture2D GetDepthData();
        public abstract Texture2D GetNormalData();

        public abstract void Destroy();

        #endregion Public Methods
    }
}
