
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A copy of an <see cref="Image"/> that is used in a <see cref="FunctionLayer"/>.
    /// <br/>   Its properties default to those of its source image.
    /// <br/>   Each property can however be overridden individually on any copy.
    /// </summary>
    public class ImageCopy
    {
        public Image Source { get; private set; } = null;


        // Shared Properties -------------------------------------------------- Shared Properties

        public bool OverrideSteps { get; set; } = false;
        public int Steps { get; set; } = 0;

        public bool OverrideMultiplier { get; set; } = false;
        public float Multiplier { get; set; } = 1f;


        // Depth Properties --------------------------------------------------- Depth Properties



        // Normal Properties -------------------------------------------------- Normal Properties

        public bool OverrideFlipX { get; set; } = false;
        public bool OverrideFlipY { get; set; } = false;
        public bool OverrideFlipZ { get; set; } = false;
        public bool FlipX { get; set; } = false;
        public bool FlipY { get; set; } = false;
        public bool FlipZ { get; set; } = false;


        public ImageCopy(Image source) { Source = source; }
    }
}
