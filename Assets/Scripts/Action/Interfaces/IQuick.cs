
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Interface used to create a quick <see cref="Action"/>.
    /// <br/>   A quick action executes after its creation.
    /// </summary>
    public interface IQuick
    {
        /// <summary>
        /// <br/>   Gets called after action creation.
        /// <br/>   Should return a boolean, which tells if action succeeded.
        /// </summary>
        bool Do();
    }
}
