
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Determines how neighbouring pixels are picked. </summary>
    public enum PixelSelectionGrowStyle
    {
        Distance,
        Adjacent,
        Neighbouring,
    }

    /// <summary> Handles the storing and modification of different selections. </summary>
    public class SelectionHandler
    {
        public List<Image> ImageSelection { get; private set; } = new();
        public List<FunctionLayer> LayerSelection { get; private set; } = new();

        // TODO: Make custom Mesh class
        public List<Object> MeshSelection { get; private set; } = new();

        /// <summary>
        /// <br/>   Selected pixels are stored as floats within range [0, 1].
        /// <br/>   The float determines how much an action affects the pixel.
        /// </summary>
        public float[,] PixelSelection { get; private set; } = new float[0, 0];
    }
}
