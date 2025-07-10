
using System;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Actions are used to interact with different parts of the application.
    /// <br/>   Action creation, disposal and updating is handled by <see cref="ActionHandler"/>.
    /// </summary>
    public class Action : IDisposable
    {
        public virtual void Dispose() { }
    }
}
