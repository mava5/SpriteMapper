
using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Actions are used to interact with different parts of the program. </summary>
    public class Action
    {
        public static ActionInfo Info => ActionInfoDictionary.Instance[typeof(ActionInfo)];
    }
}
