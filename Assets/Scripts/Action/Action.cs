
using UnityEngine;


namespace SpriteMapper
{
    public enum ActionContext
    {
        Global,
        DrawImage,
        MeshImage,
        LayerView,
    }

    public class Action
    {
        public bool IsLong => this is ILong;
        public bool IsUndoable => this is IUndoable;

        /// <summary>
        /// <br/>   Context, in which action can be performed.
        /// <br/>   Defaults to <see cref="ActionContext.Global"/> but can be overridden.
        /// </summary>
        public virtual ActionContext Context => ActionContext.Global;
    }
}
