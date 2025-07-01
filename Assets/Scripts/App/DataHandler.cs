
using System;
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


        #region Public Methods ==================================================================== Public Methods

        public Image CreateImage<T>(int width, int height) where T : Image
        {
            Image image = (T)Activator.CreateInstance(typeof(T), new object[] { width, height });

            return image;
        }

        public void RemoveImage(Image imageToRemove)
        {
            Images.Remove(imageToRemove);
            imageToRemove.Dispose();
        }

        #endregion Public Methods
    }
}
