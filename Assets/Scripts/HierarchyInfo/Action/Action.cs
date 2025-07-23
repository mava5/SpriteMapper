
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
        /// <summary> Pressed short action. </summary>
        Instant,

        /// <summary> Pressed long action. </summary>
        Toggle,

        /// <summary> Held long action. </summary>
        Hold,
    }


    /// <summary> Actions are used to interact with different parts of the application. </summary>
    public abstract class Action : IDisposable
    {
        public abstract ActionInfo Info { get; }

        /// <param name="manuallyCalled">
        /// <br/>   true:
        /// <br/>   • Called manually from <see cref="Dispose()"/>
        /// <br/>   • Safe to free unmanaged resources
        ///         (<see cref="UnityEngine.Texture2D"/>, <see cref="UnityEngine.Mesh"/>, etc.)
        /// <br/>   • Safe to free managed resources
        ///         (<see cref="int"/>, <see cref="string"/>, <see cref="System.Collections.Generic.List{T}"/>, etc.)
        /// <br/>   → Action object still exists
        /// <br/>
        /// <br/>   false:
        /// <br/>   • Called by garbage collector from action finalizer
        /// <br/>   • Safe to free unmanaged resources
        ///         (<see cref="UnityEngine.Texture2D"/>, <see cref="UnityEngine.Mesh"/>, etc.)
        /// <br/>   → Action object may not exist
        /// </param>
        internal virtual void Dispose(bool manuallyCalled) { }

        public void Dispose()
        {
            Dispose(true);

            // Prevent garbage collector from disposing object a second time
            GC.SuppressFinalize(this);
        }

        ~Action() { Dispose(false); }
    }
}
