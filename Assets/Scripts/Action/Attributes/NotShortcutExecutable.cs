
using System;


namespace SpriteMapper
{
    /// <summary> Used to modify an <see cref="Action"/> so that it cant be executed with a <see cref="Shortcut"/>. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NotShortcutExecutable : Attribute { }
}
