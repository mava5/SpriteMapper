
using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Nested empty classes for each context an action can be executed in.
    /// <br/>   Used with <see cref="UserExecutable"/> in the following way:
    /// <br/>   <c> [<see cref="UserExecutable"/>(typeof(<see cref="Global"/>)), ...)] </c>
    /// <br/>   
    /// <br/>   A child context can use its parent and ancestor contexts' actions.
    /// <br/>   For example <see cref="ImageEditor.DrawImage"/> also contains <see cref="ImageEditor"/> actions.
    /// </summary>
    public static class Context
    {
        #region Hierarchy ========================================================================= Hierarchy

        public class Global : CM<Global> { }

        public class ImageEditor : CM<ImageEditor>
        {
            public class DrawImage : CM<DrawImage> { }

            public class MeshImage : CM<MeshImage> { }
        }

        public class ImageList : CM<ImageList> { }

        public class LayerList : CM<LayerList> { }

        public class Properties : CM<Properties> { }


        /// <summary>
        /// <br/>   Inherited by each <see cref="Context"/>.
        /// <br/>   Contains static name for context.
        /// </summary>
        public class CM<T>
        {
            public static string Name => ToName(typeof(T));
        }

        #endregion Hierarchy


        #region Properties ======================================================================== Properties

        public static string Default => typeof(Global).FullName;

        public static List<string> All
        {
            get
            {
                if (all.Count == 0)
                {
                    foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        if (!type.IsClass || string.IsNullOrEmpty(type.FullName) ||
                            !type.FullName.StartsWith("SpriteMapper.Context+")) { continue; }

                        all.Add(ToName(type));
                    }
                }

                return all;
            }
        }

        private static List<string> all = new();

        #endregion Properties


        #region Public Methods ==================================================================== Public Methods

        /// <summary>
        /// <br/>   Converts given <see cref="Context"/> type to a context name.
        /// <br/>   For example: Context.ImageEditor.DrawImage = "ImageEditor/DrawImage"
        /// </summary>
        public static string ToName(Type actionContext)
        {
            return actionContext.FullName[actionContext.FullName.IndexOf("+")..].Replace("+", "/");
        }

        #endregion
    }

    /// <summary>
    /// <br/>   Attached to an <see cref="Action"/> turning it user executable.
    /// <br/>   Action is executed with an <see cref="Shortcut"/>, while in correct <see cref="Context"/>.
    /// <br/>   The current context is defined by the currently focused <see cref="Panel"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UserExecutable : Attribute
    {
        public readonly string ActionContext;


        public UserExecutable(Type actionContext = null)
        {
            if (actionContext == null)
            {
                // Use default context if none is given
                ActionContext = Context.Default;
            }
            else if (!actionContext.FullName.Contains(typeof(Context).FullName))
            {
                Debug.LogWarning($"Action context \"{actionContext}\" is invalid!");
                ActionContext = Context.Default;
            }
            else
            {
                ActionContext = Context.ToName(actionContext);
            }
        }
    }
}
