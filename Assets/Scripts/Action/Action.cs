
using UnityEngine;


namespace SpriteMapper
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ActionContext : System.Attribute
    {
        public Context Context { get; private set; }

        public ActionContext(Context context) { Context = context; }
    }

    /// <summary> Actions are used to interact with different parts of the program. </summary>
    public class Action
    {
        public static ActionInfo Info => ActionInfoDictionary.Instance[typeof(ActionInfo)];
    }
}
