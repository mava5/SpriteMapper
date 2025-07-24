
using UnityEngine;


namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(false, true, ActionShortcutState.Exists, ActionDescendantUsability.Full)]
        public class CancelLongActions : ShortAction
        {
            public override bool Do() { Debug.Log("Canceled"); App.Actions.CancelLongActions(); return true; }
        }
    }
}
