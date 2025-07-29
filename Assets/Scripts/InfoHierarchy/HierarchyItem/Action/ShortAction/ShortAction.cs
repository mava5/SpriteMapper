
namespace SpriteMapper
{
    /// <summary> <see cref="Action"/> that begins and ends within one frame. </summary>
    public abstract class ShortAction : Action
    {
        /// <summary> Executes action. </summary>
        /// <returns> <see cref="bool"/>: Tells whether action succeeded. </returns>
        public abstract bool Do();
    }
}
