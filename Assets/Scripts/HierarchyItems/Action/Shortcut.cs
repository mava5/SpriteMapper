
using System;


namespace SpriteMapper
{
    /// <summary> Shortcut used by an <see cref="UserExecutable"/> <see cref="Action"/>. </summary>
    [Serializable]
    public class Shortcut
    {
        public bool Shift = false;
        public bool Ctrl = false;
        public bool Alt = false;
        public string Binding = "";

        /// <summary> User readable view of shortcut. </summary>
        public string View =>
            (Shift ? "Shift + " : "") +
            (Ctrl ? "Ctrl + " : "") +
            (Alt ? "Alt + " : "") +
            (Binding.Contains("/") ? Binding.Split("/")[1] : "");

        /// <summary> Input System readable binding. </summary>
        // Code help from: https://stackoverflow.com/a/21755933
        public string InputBinding =>
            string.IsNullOrEmpty(Binding) ?
                "" :
                (Binding.Length == 1 ?
                    char.ToLower(Binding[0]).ToString() :
                    char.ToLower(Binding[0]) + Binding[1..]);


        public override bool Equals(object obj)
        {
            Shortcut other = obj as Shortcut;

            return other != null &&
                Binding == other.Binding &&
                Shift == other.Shift &&
                Ctrl == other.Ctrl &&
                Alt == other.Alt;
        }

        // Override GetHashCode() so that same shortcuts occupy same key in a dictionary
        // This helps with storing and checking unfinished actions based on their shortcut in ActionHandler
        public override int GetHashCode()
        {
            return HashCode.Combine(Shift, Ctrl, Alt, Binding);
        }
    }
}
