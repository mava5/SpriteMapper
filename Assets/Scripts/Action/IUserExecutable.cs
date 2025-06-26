
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Interface used to create actions, that the user can execute with shortcuts.
    /// <br/>   These actions have a shortcut assigned to them and thus appear in the rebinding menu.
    /// </summary>
    public interface IUserExecutable
    {
        /// <summary>
        /// <br/>   Contains information for the context in which user can execute the action.
        /// <br/>   Also contains the shortcut, with which the action is executed.
        /// </summary>
        public UserActionInfo Info { get; set; }
    }
}
