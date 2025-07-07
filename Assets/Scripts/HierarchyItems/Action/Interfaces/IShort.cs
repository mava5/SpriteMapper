
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Interface used to create a short <see cref="Action"/>.
    /// <br/>   A short action executes after its creation.
    /// </summary>
    public interface IShort
    {
        /// <summary>
        /// <br/>   Gets called after action creation.
        /// <br/>   Should return a boolean, which tells if action succeeded.
        /// </summary>
        bool Do();
    }
}
