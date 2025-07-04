
using System;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Determines in what order each <see cref="Action"/> with same <see cref="Shortcut"/> is done.
    /// <br/>   If a higher priority action hasn't finished, the lower priority ones get ignored.
    /// </summary>
    public enum PriorityLevel
    {
        Low,
        Normal,
        High,
    }

    /// <summary>
    /// <br/>   Makes <see cref="Action"/> user executable in a specified <see cref="Context"/>.
    /// <br/>   Action is executed with an <see cref="Shortcut"/>, while in correct context.
    /// <br/>   The current context is defined by the currently focused <see cref="Panel"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionUsedIn : Attribute
    {
        public readonly string ActionContext;
        public readonly int Priority;


        public ActionUsedIn(Type context = null, PriorityLevel priority = PriorityLevel.Normal)
        {
            if (context == null)
            {
                // Use default context if none is given
                ActionContext = Context.Default;
            }
            else if (!context.FullName.Contains(typeof(Context).FullName))
            {
                Debug.LogWarning($"Action's context \"{context}\" is invalid!");
                ActionContext = Context.Default;
            }
            else
            {
                ActionContext = Context.ToName(context);
            }
        }
    }
}
