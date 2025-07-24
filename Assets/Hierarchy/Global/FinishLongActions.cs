
using UnityEngine;


namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(false, true, ActionShortcutState.Exists, ActionDescendantUsability.Full)]
        public class FinishLongActions : ShortAction
        {
            public override bool Do() { Debug.Log("Finished"); App.Actions.FinishLongActions(); return true; }
        }
    }
}
