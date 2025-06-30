
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Image type that:
    /// <br/>   • derives data from a 2D drawing,
    /// <br/>   • can contain either depth or normal data,
    /// <br/>   • can be converted into from other image types
    /// </summary>
    public class DrawImage : Image
    {
        public Texture2D DepthTexture { get; internal set; } = null;
        public Texture2D NormalTexture { get; internal set; } = null;


        #region Initialization ========================================================== Initialization

        public DrawImage(int width, int height) : base(width, height)
        {
            Type = ImageType.Draw;
        }

        #endregion Initialization


        #region Public Methods ========================================================== Public Methods

        public override Texture2D GetDepthData()
        {
            return null;
        }

        public override Texture2D GetNormalData()
        {
            return null;
        }

        public override void Destroy()
        {
            GameObject.Destroy(DepthTexture);
            GameObject.Destroy(NormalTexture);
        }

        #endregion Public Methods
    }
}
