
using System;


namespace SpriteMapper
{
    /// <summary> Determines how an <see cref="Action"/> is executed based on input. </summary>
    public enum ActionInputType
    {
        /// <summary>
        /// <br/>   <see cref="Action"/> is executed when <see cref="Shortcut"/> is pressed down.
        /// <br/>   (or released when in solvable conflict)
        /// </summary>
        Pressed,

        /// <summary> <see cref="Action"/> is executed while <see cref="Shortcut"/> is held down. </summary>
        Held,
    }

    /// <summary> Determines how long an <see cref="Action"/> execution lasts. </summary> 
    public enum ActionDuration
    {
        /// <summary> Action is executed within one frame. </summary>
        Short,

        /// <summary> Action is executed over multiple frames. </summary>
        Long,
    }

    /// <summary> Determines the base behaviour of an <see cref="Action"/>. </summary>
    public enum ActionBehaviourType
    {
        /// <summary> <see cref="InstantAction"/> </summary>
        Instant,

        /// <summary> <see cref="ToggleAction"/> </summary>
        Toggle,

        /// <summary> <see cref="HoldAction"/> </summary>
        Hold,
    }


    /// <summary> Actions are used to interact with different parts of the application. </summary>
    public abstract class Action : IDisposable
    {
        public abstract ActionInfo Info { get; }

        public virtual void Dispose() { }
    }
}
