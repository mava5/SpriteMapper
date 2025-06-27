
namespace SpriteMapper
{
    /// <summary> Interface used to create actions, that the user can execute with shortcuts. </summary>
    public interface IUserExecutable
    {
        /// <summary>  </summary>
        public static string Description => "";

        /// <summary>
        /// <br/>   The key or key combination the user presses to execute this action.
        /// <br/>   Contains <see cref="Shortcut.Binding"/>, which is an action binding for the Input System.
        /// </summary>
        public static Shortcut Shortcut { get; private set; }

        /// <summary> Rebinds the action's shortcut. </summary>
        public static void Rebind(Shortcut newShortcut) { Shortcut = newShortcut; }
    }
}
