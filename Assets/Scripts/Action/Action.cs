
using System;

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
        public static string Default => typeof(Global).FullName;

        public static class Global { }

        public static class ImageEditor
        {
            public static class DrawImage { }

            public static class MeshImage { }
        }

        public static class ImageList { }

        public static class LayerList { }

        public static class Properties { }
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
                Debug.LogWarning($"Action context {actionContext.FullName} is invalid!");
                ActionContext = Context.Default;
            }
            else
            {
                ActionContext = actionContext.FullName;
            }
        }
    }

    /// <summary> Actions are used to interact with different parts of the application. </summary>
    public class Action : IDisposable
    {
        public virtual void Dispose() { }
    }
}
