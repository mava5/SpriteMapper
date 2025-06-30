
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Image type that:
    /// <br/>   • derives data from a collection of 3D meshes,
    /// <br/>   • contains both depth and normal data
    /// </summary>
    public class MeshImage : Image
    {




        #region Initialization ========================================================== Initialization

        public MeshImage(int width, int height) : base(width, height)
        {
            Type = ImageType.Mesh;
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
            
        }

        #endregion Public Methods
    }
}
