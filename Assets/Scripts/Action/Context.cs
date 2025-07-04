
using System;
using System.Reflection;
using System.Collections.Generic;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Nested empty classes for each context an action can be executed in.
    /// <br/>   Used with <see cref="ActionUsedIn"/> or <see cref="ActionUsedWith"/> in the following way:
    /// <br/>   <c> [<see cref="ActionUsedIn"/>(typeof(<see cref="Global"/>)), ...)] </c>
    /// <br/>   <c> [<see cref="ActionUsedWith"/>(typeof(<see cref="Viewport.DrawImage.Tools.Move"/>)), ...)] </c>
    /// <br/>   
    /// <br/>   A child context can use its parent and ancestor contexts' actions.
    /// <br/>   For example <see cref="Viewport.DrawImage"/> also contains <see cref="Viewport"/> actions.
    /// </summary>
    public static class Context
    {
        #region Hierarchy ========================================================================= Hierarchy

        /// <summary> Each global <see cref="Action"/> can be used anywhere. </summary>
        public class Global : CN<Global> { }

        /// <summary> Child contexts make use of a 3D viewport with some navigation tools. </summary>
        public class Viewport : CN<Viewport>
        {
            public class DrawImage : CN<DrawImage>
            {
                public class Tools
                {
                    public class Move : CN<Move> { }
                    public class Rotate : CN<Rotate> { }
                    public class Scale : CN<Scale> { }
                }
            }

            public class MeshImage : CN<MeshImage>
            {
                public class Tools
                {
                    public class Move : CN<Move> { }
                    public class Rotate : CN<Rotate> { }
                    public class Scale : CN<Scale> { }
                }
            }

            public class Preview : CN<Preview> { }
        }

        public class ImageList : CN<ImageList> { }

        public class LayerList : CN<LayerList> { }

        public class Properties : CN<Properties> { }

        #endregion Hierarchy


        #region Properties ======================================================================== Properties

        public static string Default => typeof(Global).FullName;

        public static List<string> All
        {
            get
            {
                List<string> all = new();

                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (!type.IsClass || string.IsNullOrEmpty(type.FullName) ||
                        !type.FullName.StartsWith("SpriteMapper.Context+")) { continue; }

                    all.Add(ToName(type));
                }

                return all;
            }
        }

        #endregion Properties


        #region Public Methods ==================================================================== Public Methods

        /// <summary>
        /// <br/>   Converts given <see cref="Context"/> type to a context name.
        /// <br/>   For example: Context.ImageEditor.DrawImage = "ImageEditor/DrawImage"
        /// </summary>
        public static string ToName(Type context)
        {
            return context.FullName[context.FullName.IndexOf("+")..].Replace("+", "/");
        }

        #endregion
    }


    /// <summary>
    /// <br/>   Inherited by each <see cref="Context"/>.
    /// <br/>   Contains static name property for context.
    /// </summary>
    public class CN<T>
    {
        public static string Name => Context.ToName(typeof(T));
    }
}
