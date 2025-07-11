
using System;

namespace SpriteMapper
{
    /// <summary> Makes <see cref="Action"/> executable while a parent <see cref="Tool"/> is equipped. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UsedWhileToolEquipped : Attribute { }

    /// <summary> Makes <see cref="Action"/> executable while a parent <see cref="Tool"/> is active. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UsedWhileToolActive : Attribute { }
}
