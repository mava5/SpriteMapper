
using UnityEngine;


namespace SpriteMapper.Actions
{
    [InstantActionSettings(false, true, ActionShortcutState.Rebindable, ActionDescendantUsability.Full)]
    public class FinishLongActions : ShortAction
    {
        public override bool Do() { Debug.Log("Finished"); App.Actions.FinishLongActions(); return true; }
    }
}
