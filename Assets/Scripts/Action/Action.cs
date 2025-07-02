
using System;


namespace SpriteMapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionContext : Attribute
    {
        public Context Context { get; private set; }

        public ActionContext(Context context = Context.Unassigned) { Context = context; }
    }

    /// <summary> Actions are used to interact with different parts of the application. </summary>
    public class Action : IDisposable
    {
        public virtual void Dispose() { }
    }
}
