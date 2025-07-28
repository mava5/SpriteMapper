
using UnityEngine;


namespace SpriteMapper.Actions
{
    [InstantActionSettings(false, true, ActionShortcutState.Rebindable, ActionDescendantUsability.Full)]
    public class CancelLongActions : ShortAction
    {
        public override bool Do() { Debug.Log("Canceled"); App.Actions.CancelLongActions(); return true; }
    }
}
