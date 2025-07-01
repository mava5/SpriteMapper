
using System;

namespace SpriteMapper
{
    /// <summary> Determines the <see cref="SpriteMapper.Context"/> in which <see cref="Action"/> can be used </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ActionContext : System.Attribute
    {
        public Context Context { get; private set; }

        public ActionContext(Context context) { Context = context; }
    }

    /// <summary> Actions are used to interact with different parts of the application. </summary>
    public class Action : IDisposable
    {
        public virtual void Dispose() { }
    }
}
