
using System;


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

    /// <summary> Used to give an <see cref="Action"/> a specific <see cref="PriorityLevel"/>. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionPriority : Attribute
    {
        public readonly PriorityLevel Priority;

        public ActionPriority(PriorityLevel priority = PriorityLevel.Normal) { Priority = priority; }
    }
}
