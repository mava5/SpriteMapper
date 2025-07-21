
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   <see cref="Action"/> that begins, lasts multiple frames and either gets cancelled or finished.
    /// <br/>   Can be made to use its own context when active.
    /// </summary>
    public abstract class LongAction : Action
    {
        /// <summary> Begins action execution. </summary>
        /// <returns> <see cref="bool"/>: Tells whether action began successfully. </returns>
        public abstract bool Begin();

        /// <summary> Gets called each MonoBehaviour Update(). </summary>
        public abstract void Update();

        /// <summary> Discards any changes the action would have done. </summary>
        public abstract void Cancel();

        /// <summary> Applies changes done by action. </summary>
        public abstract void Finish();
    }
}
