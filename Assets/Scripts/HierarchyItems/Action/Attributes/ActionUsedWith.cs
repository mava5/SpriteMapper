
using System;

using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Makes <see cref="Action"/> user executable with a specified tool equipped. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionUsedWith : Attribute
    {
        //public readonly string ActionContext;


        //public ActionUsedWith(Type context)
        //{
        //    if (context == null)
        //    {
        //        Debug.LogWarning($"Action's tool context is null!");
        //    }
        //    else if (!context.FullName.Contains(typeof(Context).FullName))
        //    {
        //        Debug.LogWarning($"Action's tool context \"{context}\" is invalid!");
        //    }
        //    else
        //    {
        //        ActionContext = Context.ToName(context);
        //    }
        //}
    }
}
