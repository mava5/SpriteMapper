
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles the modificiation of different app related data.
    /// <br/>   For example <see cref="Image"/> and <see cref="FunctionLayer"/> data.
    /// </summary>
    public class DataHandler
    {
        public List<Image> Images { get; private set; } = new();
        public List<FunctionLayer> FunctionLayers { get; private set; } = new();
    }
}
